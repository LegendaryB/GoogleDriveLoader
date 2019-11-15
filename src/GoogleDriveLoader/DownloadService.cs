using GoogleDriveLoader.Models;
using GoogleDriveLoader.Presentation;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GoogleDriveLoader
{
    internal class DownloadService
    {
        private readonly DriveServiceWrapper _driveServiceWrapper;
        private readonly int _maxParallelDownloads;

        public DownloadService(int maxParallelDownloads)
        {
            _driveServiceWrapper = new DriveServiceWrapper();
            _maxParallelDownloads = maxParallelDownloads;
        }

        internal async Task DownloadAsync(string directoryUrl, CancellationToken token)
        {
            var folder = await _driveServiceWrapper.GetDriveDirectoryAsync(directoryUrl, token);

            if (!folder.ContainsFiles)
                return;

            folder.LocalPath = Filesystem.CreateDirectory(folder.Name);
            ExceptAlreadyDownloadedFiles(folder);

            Parallel.ForEach(folder.Files, new ParallelOptions { MaxDegreeOfParallelism = _maxParallelDownloads }, (file) =>
            {
                var resource = _driveServiceWrapper.GetDriveFile(file.Id);

                using var stream = Filesystem.CreateFile(file.Name, folder.LocalPath);
                using var downloadProgressBar = new DownloadProgressBar(file, resource.MediaDownloader);

                var result = resource.DownloadWithStatus(stream);
            });
        }

        private void ExceptAlreadyDownloadedFiles(DriveDirectory driveDirectory)
        {
            var hashes = Filesystem.GetFileHashes(driveDirectory.LocalPath);
            var filesToRemove = new List<DriveFile>();

            foreach (var hash in hashes)
            {
                var file = driveDirectory.Files.FirstOrDefault(f => f.Checksum == hash);

                if (file != null)
                    filesToRemove.Add(file);
            }

            driveDirectory.Files = driveDirectory.Files.Except(filesToRemove).ToList();
        }
    }
}
