using System;
using System.Threading;
using System.Windows.Forms;

namespace GoogleDriveLoader
{
    internal static class WindowHandler
    {
        // todo introduce interface for window
        internal static ClipboardNotificationWindow CreateWindow(
            CancellationToken token)
        {
            var window = new ClipboardNotificationWindow(token);

            var thread = new Thread(() =>
            {
                Application.Run(window);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            CancelableThread.Create(thread, token);

            return window;
        }
    }
}
