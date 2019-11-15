using System.Threading;
using System.Threading.Tasks;

namespace GoogleDriveLoader
{
    internal static class DownloadQueueObserver
    {
        private static DownloadService _mediaService;

        internal static async Task ObserveAsync(int maxParallelDownloads, CancellationToken token)
        {
            _mediaService = new DownloadService(maxParallelDownloads);

            while (!token.IsCancellationRequested)
            {
                if (!DownloadQueue.TryDequeue(out var directoryUrl) || string.IsNullOrWhiteSpace(directoryUrl))
                {
                    await Task.Delay(1000);
                    continue;
                }

                await _mediaService.DownloadAsync(directoryUrl, token);
            }
        }
    }
}
