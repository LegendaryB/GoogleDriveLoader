using System;
using System.Collections.Concurrent;

namespace GoogleDriveLoader
{
    internal static class MediaQueue
    {
        internal static Action ItemAdded;

        private static ConcurrentQueue<string> Items;

        static MediaQueue()
        {
            Items = new ConcurrentQueue<string>();

            ConsoleOutput.WriteLine("Media queue initialization successful.");
        }

        internal static void Enqueue(string item)
        {
            Items.Enqueue(item);
            ItemAdded?.Invoke();
        }

        internal static bool TryDequeue(out string item)
        {
            return Items.TryDequeue(out item);
        }
    }
}
