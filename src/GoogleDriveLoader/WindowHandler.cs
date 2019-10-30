using System;
using System.Threading;
using System.Windows.Forms;

namespace GoogleDriveLoader
{
    internal static class WindowHandler
    {
        private static ClipboardNotificationWindow window;

        internal static void CreateWindow(
            CancellationToken token)
        {
            window = new ClipboardNotificationWindow(token);
            Application.Run(window);

            //var thread = new Thread(() =>
            //{
            //    Application.Run(window);
            //});

            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();

            //token.Register(() =>
            //{
            //    Console.WriteLine(thread.IsAlive);
            //});
        }
    }
}
