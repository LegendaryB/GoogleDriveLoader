using System.Collections.Concurrent;

namespace GoogleDriveLoader
{
    internal static class DownloadQueue
    {
        private static ConcurrentQueue<string> Items;

        static DownloadQueue()
        {
            Items = new ConcurrentQueue<string>();
        }

        internal static void Enqueue(string item)
        {
            Items.Enqueue(item);
        }

        internal static bool TryDequeue(out string item)
        {
            return Items.TryDequeue(out item);
        }
    }
}
