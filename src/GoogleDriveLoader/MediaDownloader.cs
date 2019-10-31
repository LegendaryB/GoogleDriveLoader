namespace GoogleDriveLoader
{
    internal static class MediaDownloader
    {
        private const int MAX_PARALLEL_DOWNLOADS = 3;

        private static string outputFolder;
        private static int jobCount;

        internal static void ExecuteInBackground(AppOptions options)
        {
            outputFolder = options.OutputFolder;
            MediaQueue.ItemAdded += OnMediaQueueItemAdded;
        }

        private static void OnMediaQueueItemAdded()
        {
            if (!MediaQueue.TryDequeue(out var link))
                return;

            if (jobCount == MAX_PARALLEL_DOWNLOADS)
            {
                RequeueMediaItem(link);
                return;
            }

            DownloadMedia(link);
        }

        private static void DownloadMedia(string link)
        {
            jobCount++;



            jobCount--;
        }

        private static void RequeueMediaItem(string item)
        {
            MediaQueue.ItemAdded -= OnMediaQueueItemAdded;
            MediaQueue.Enqueue(item);     
            MediaQueue.ItemAdded += OnMediaQueueItemAdded;
        }
    }
}
