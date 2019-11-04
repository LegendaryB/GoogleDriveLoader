﻿using Google.Apis.Auth.OAuth2;
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
                if (!MediaQueue.TryDequeue(out var link) || string.IsNullOrWhiteSpace(link))
                {
                    await Task.Delay(1000);
                    continue;
                }

                await DownloadAsync(link);
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

        private async Task DownloadAsync(string link)
        {
            var folder = await GetFolderAsync(link);

            if (folder == null)
                return;

            var files = await GetFilesAsync(folder);

            if (!files.Any())
                return;

            var localFolder = Path.Combine(_outputFolder, folder.Name);
            Directory.CreateDirectory(localFolder);

            Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelDownloads }, (file) =>
            {
                var resource = _driveService.Files.Get(file.Id);

                using (var stream = new FileStream(Path.Combine(localFolder, file.Name), FileMode.Create))
                {
                    var result = resource.DownloadWithStatus(stream);
                }
            });
        }

        private async Task<GoogleDriveFile> GetFolderAsync(string link)
        {
            var driveFolderId = link.Split('/').Last();

            var fileRequest = _driveService.Files.Get(driveFolderId);
            fileRequest.SupportsAllDrives = true;
            fileRequest.SupportsTeamDrives = true;

            var file = await fileRequest.ExecuteAsync();
            return file;
        }
        private async Task<IList<GoogleDriveFile>> GetFilesAsync(GoogleDriveFile folder)
        {
            var request = _driveService.Files.List();
            request.IncludeItemsFromAllDrives = true;
            request.IncludeTeamDriveItems = true;
            request.SupportsAllDrives = true;
            request.SupportsTeamDrives = true;
            request.Fields = "files(id, name, size, trashed, md5Checksum)";
            request.Q = $"'{folder.Id}' in parents and trashed=false";

            return (await request.ExecuteAsync()).Files;
        }

        private double AsMegabytes(long? bytes)
        {
            if (bytes == null)
                return 0;

            return Math.Round(bytes.Value / 1024f / 1024f);
        }
    }
}
