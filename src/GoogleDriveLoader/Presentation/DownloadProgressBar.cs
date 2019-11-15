using System;

using Konsole;
using Google.Apis.Download;
using GoogleDriveLoader.Helpers;
using GoogleDriveLoader.Models;

namespace GoogleDriveLoader.Presentation
{
    internal class DownloadProgressBar : ProgressBar,
        IDisposable
    {
        private readonly DriveFile _file;
        private readonly IMediaDownloader _mediaDownloader;

        public DownloadProgressBar(DriveFile file, IMediaDownloader mediaDownloader) 
            : base(file.Size, file.Name.Length, '─')
        {
            _file = file;

            Refresh(0, _file.Name);

            _mediaDownloader = mediaDownloader;
            _mediaDownloader.ProgressChanged += OnProgressChanged;
        }

        private void OnProgressChanged(IDownloadProgress progress)
        {
            var megabytesDownloaded = FileSizeCalculator.ToMegabyte(progress.BytesDownloaded);
            Refresh(megabytesDownloaded, _file.Name);
        }

        public void Dispose()
        {
            _mediaDownloader.ProgressChanged -= OnProgressChanged;
        }
    }
}
