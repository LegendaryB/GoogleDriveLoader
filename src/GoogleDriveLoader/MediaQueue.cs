using System;
using System.Collections.Concurrent;

namespace GoogleDriveLoader
{
    internal static class MediaQueue
    {
        private static ConcurrentQueue<string> Items;

        static MediaQueue()
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
