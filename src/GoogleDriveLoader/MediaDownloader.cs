using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using GoogleDriveFile = Google.Apis.Drive.v3.Data.File;

namespace GoogleDriveLoader
{
    internal class MediaDownloader
    {
        private readonly DriveService _driveService;
        private readonly int _maxParallelDownloads;
        private readonly string _outputFolder;
        private readonly CancellationToken _token;

        private static int taskCount;

        public MediaDownloader(AppOptions options, CancellationToken token)
        {
            _maxParallelDownloads = options.MaxParallelDownloads;
            _outputFolder = options.OutputFolder;
            _token = token;

            _driveService = InitializeDriveService();
        }

        public async Task ListenAsync()
        {
            while (!_token.IsCancellationRequested)
            {
                if (!MediaQueue.TryDequeue(out var link))
                {
                    await Task.Delay(1000);
                    continue;
                }

                await DownloadMedia(link);
            }
        }

        private DriveService InitializeDriveService()
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new string[] { DriveService.Scope.DriveReadonly },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive Loader",
            });
        }

        private async Task DownloadMedia(string link)
        {
            var folder = await RetrieveFolder(link);

            if (folder == null)
            {
                ConsoleOutput.WriteLine("Folder download aborted and removed from queue.");
                return;
            }

            var files = await RetrieveFolderFiles(folder);

            if (!files.Any())
            {
                ConsoleOutput.WriteLine($"Folder '{folder.Name}' contains no data. Aborted and removed from queue.");
                return;
            }

            ConsoleOutput.WriteLine($"Found {files.Count} files in folder '{folder.Name}'.");

            foreach (var file in files)
            {
                while (taskCount == _maxParallelDownloads)
                    await Task.Delay(500);

                var resource = _driveService.Files.Get(file.Id);

                taskCount++;

                Task.Run(async () =>
                {
                    var localFolder = Path.Combine(_outputFolder, folder.Name);
                    Directory.CreateDirectory(localFolder);

                    using (var stream = new FileStream(Path.Combine(localFolder, file.Name), FileMode.Create))
                    {
                        ConsoleOutput.WriteLine($"Downloading '{file.Name}' ({AsMegabytes(file.Size)} MB)..");
                        var result = await resource.DownloadAsync(stream);
                        taskCount--;
                    }
                });
            }
        }

        private async Task<GoogleDriveFile> RetrieveFolder(string link)
        {
            var driveFolderId = link.Split('/').Last();

            try
            {
                var fileRequest = _driveService.Files.Get(driveFolderId);
                fileRequest.SupportsAllDrives = true;
                fileRequest.SupportsTeamDrives = true;

                var file = await fileRequest.ExecuteAsync();
                ConsoleOutput.WriteLine($"Found folder - id: '{driveFolderId}' - name: {file.Name}");
                return file;
            }
            catch
            {
                ConsoleOutput.WriteLine($"Could not find folder with id '{driveFolderId}'.");
            }

            return null;
        }

        private async Task<IList<GoogleDriveFile>> RetrieveFolderFiles(GoogleDriveFile folder)
        {
            var request = _driveService.Files.List();
            request.IncludeItemsFromAllDrives = true;
            request.IncludeTeamDriveItems = true;
            request.SupportsAllDrives = true;
            request.SupportsTeamDrives = true;
            request.Fields = "nextPageToken, files(id, name, capabilities, size, mimeType)";
            request.Q = $"'{folder.Id}' in parents and trashed=false";

            return (await request.ExecuteAsync()).Files;
        }

        private static double AsMegabytes(long? bytes)
        {
            if (bytes == null)
                return 0;

            return Math.Round(bytes.Value / 1024f / 1024f);
        }
    }
}
