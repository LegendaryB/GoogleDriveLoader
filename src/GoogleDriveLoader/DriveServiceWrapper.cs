using GoogleDriveLoader.Models;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using DriveSDKFile = Google.Apis.Drive.v3.Data.File;

namespace GoogleDriveLoader
{
    internal class DriveServiceWrapper
    {
        private readonly DriveService _service;

        internal DriveServiceWrapper()
        {
            _service = Initialize();
        }        

        internal async Task<DriveDirectory> GetDriveDirectoryAsync(string directoryUrl, CancellationToken token)
        {
            var id = GetDriveDirectoryId(directoryUrl);

            var request = _service.Files.Get(id);
            request.SupportsAllDrives = true;
            request.SupportsTeamDrives = true;

            var response = await request.ExecuteAsync(token);

            return new DriveDirectory
            {
                Id = id,
                Name = response.Name,
                Files = await GetDriveDirectoryFilesAsync(id, token)
            };
        }

        internal FilesResource.GetRequest GetDriveFile(string fileId)
        {
            return _service.Files.Get(fileId);
        }

        private DriveService Initialize()
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

        private async Task<IList<DriveFile>> GetDriveDirectoryFilesAsync(string directoryId, CancellationToken token)
        {
            var fileList = new List<DriveSDKFile>();
            var nextPageToken = string.Empty;

            do
            {
                var request = _service.Files.List();
                request.PageToken = nextPageToken;
                request.IncludeItemsFromAllDrives = true;
                request.IncludeTeamDriveItems = true;
                request.SupportsAllDrives = true;
                request.SupportsTeamDrives = true;
                request.Fields = "nextPageToken, files(id, name, size, trashed, mimeType, md5Checksum)";
                request.Q = $"'{directoryId}' in parents and mimeType != 'application/vnd.google-apps.folder' and trashed=false";

                var result = await request.ExecuteAsync(token);
                fileList.AddRange(result.Files);

                nextPageToken = result.NextPageToken;
            }
            while (!string.IsNullOrWhiteSpace(nextPageToken) && !token.IsCancellationRequested);

            return fileList.Select(file =>
            {
                return new DriveFile(
                    file.Id,
                    file.Name,
                    file.Size,
                    file.Md5Checksum);
            }).ToList();
        }

        private string GetDriveDirectoryId(string directoryUrl)
        {
            return directoryUrl.Split('/').Last();
        }
    }
}
