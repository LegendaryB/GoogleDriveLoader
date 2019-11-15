using GoogleDriveLoader.Native;
using GoogleDriveLoader.Presentation;
using GoogleDriveLoader.Models.Configuration;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleDriveLoader
{
    public static class App
    {
        private static readonly CancellationTokenSource cts = 
            new CancellationTokenSource();

        private static async Task Main()
        {
            var thread = new Thread(() =>
            {
                Application.Run(new ClipboardNotificationWindow(cts));
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            InitializeConsole();

            var options = AppOptions.Get();
            Filesystem.Initialize(options.DownloadRootDirectory);

            await DownloadQueueObserver.ObserveAsync(options.MaxParallelDownloads, cts.Token);
        }

        private static void InitializeConsole()
        {
            Kernel32.AllocConsole();
            Console.Title = "Google Drive Loader";
        }
    }
}
