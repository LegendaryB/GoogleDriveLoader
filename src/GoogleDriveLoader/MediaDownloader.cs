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

        private static int taskCount;

        private MediaDownloader(AppOptions options)
        {
            _maxParallelDownloads = options.MaxParallelDownloads;
            _outputFolder = options.OutputFolder;

            _driveService = InitializeDriveService();

            MediaQueue.ItemAdded += OnMediaQueueItemAdded;
        }

        public static MediaDownloader AttachToMediaQueue(AppOptions options)
        {
            return new MediaDownloader(options);
        }

        private DriveService InitializeDriveService()
        {
            // todo: test with unauthorized app
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
                ApplicationName = "Drive API .NET Quickstart",
            });
        }

        private void OnMediaQueueItemAdded()
        {
            if (!MediaQueue.TryDequeue(out var link))
                return;

            //if (taskCount == _maxParallelDownloads)
            //{
            //    RequeueMediaItem(link);
            //    return;
            //}

            Task.Run(async () =>
            {
                await DownloadMedia(link);
            });
        }

        private async Task DownloadMedia(string link)
        {
            taskCount++;

            var files = await GetFilesInFolder(link);
            
            foreach(var file in files)
            {
                var fileResource = _driveService.Files.Get(file.Id);

                using (var stream = new FileStream(Path.Combine(_outputFolder, file.Name), FileMode.Create))
                {
                    ConsoleOutput.WriteLine($"Downloading '{file.Name}' ({BytesToMegabytes(file.Size)} MB)..");
                    var result = await fileResource.DownloadAsync(stream);
                    taskCount--;
                }
            }
        }

        private static double BytesToMegabytes(long? bytes)
        {
            if (bytes == null)
                return 0;

            return Math.Round(bytes.Value / 1024f / 1024f);
        }

        private async Task<IEnumerable<GoogleDriveFile>> GetFilesInFolder(string link)
        {
            var driveFolderId = link.Split('/').Last();

            var request = _driveService.Files.List();
            request.IncludeItemsFromAllDrives = true;
            request.SupportsAllDrives = true;
            request.Fields = "nextPageToken, files(id, name, capabilities, size)";
            request.Q = $"'{driveFolderId}' in parents and trashed=false";

            var files = (await request.ExecuteAsync()).Files;
            ConsoleOutput.WriteLine($"Found {files.Count} items in folder '{driveFolderId}'.");

            return files;
        }

        //private void RequeueMediaItem(string item)
        //{
        //    MediaQueue.ItemAdded -= OnMediaQueueItemAdded;
        //    MediaQueue.Enqueue(item);     
        //    MediaQueue.ItemAdded += OnMediaQueueItemAdded;
        //}
    }
}
