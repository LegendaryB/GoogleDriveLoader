using System;
using System.Threading;

namespace GoogleDriveLoader
{
    internal class CancelableThread
    {
        private readonly Thread _thread;

        private CancelableThread(Thread thread) 
        {
            _thread = thread;
        }

        internal static CancelableThread Create(Thread thread)
        {
            if (thread == null)
                throw new ArgumentNullException(nameof(thread));

            if (!thread.IsAlive)
                throw new InvalidOperationException(nameof(thread));

            return new CancelableThread(thread);
        }
    }
}
