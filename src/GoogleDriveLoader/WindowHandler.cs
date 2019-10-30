using System;
using System.Threading;
using System.Windows.Forms;

namespace GoogleDriveLoader
{
    internal static class WindowHandler
    {
        private static ClipboardNotificationWindow window;
        private static Thread thread;

        internal static void CreateWindow(
            CancellationToken token)
        {
            window = new ClipboardNotificationWindow();

            thread = new Thread(() =>
            {
                Application.Run(window);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            token.Register(DestroyWindow);
        }

        private static void DestroyWindow()
        {
            window.Invoke(new Action(window.Close));
        }
    }
}
