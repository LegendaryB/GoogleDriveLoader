using GoogleDriveLoader.Native;

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace GoogleDriveLoader
{
    public static class App
    {
        private static AppOptions options;
        private static CancellationTokenSource cts;

        [STAThread]
        private static void Main()
        {
            InitializeConsole();

            options = AppOptions.Get();
            cts = new CancellationTokenSource();

            CreateOutputFolder();

            var downloader = new MediaDownloader(options, cts.Token);
            downloader.ListenAsync();

            Application.Run(new ClipboardNotificationWindow(cts));
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
