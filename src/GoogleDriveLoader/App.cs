using GoogleDriveLoader.Native;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleDriveLoader
{
    public static class App
    {
        private static AppOptions options;
        private static CancellationTokenSource cts;

        private static async Task Main()
        {
            var thread = new Thread(() =>
            {
                Application.Run(new ClipboardNotificationWindow(cts));
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            InitializeConsole();

            options = AppOptions.Get();
            cts = new CancellationTokenSource();

            CreateOutputFolder();

            await new MediaDownloader(options, cts.Token).ListenAsync();
        }

        private static void CreateOutputFolder()
        {
            if (string.IsNullOrWhiteSpace(options.OutputFolder))
                throw new ArgumentException(nameof(options.OutputFolder));

            if (!Directory.Exists(options.OutputFolder))
            {
                Directory.CreateDirectory(options.OutputFolder);
                return;
            }
        }

        private static void InitializeConsole()
        {
            Kernel32.AllocConsole();
            Console.Title = "Google Drive Loader";
        }
    }
}
