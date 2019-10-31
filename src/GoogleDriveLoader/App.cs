using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace GoogleDriveLoader
{
    public static class App
    {
        private static AppOptions options;
        private static CancellationTokenSource cts = new CancellationTokenSource();

        [STAThread]
        private static void Main()
        {
            options = AppOptions.Get();

            InitializeFolderStructure();

            Application.Run(new ClipboardNotificationWindow(cts));
        }

        private static void InitializeFolderStructure()
        {
            if (string.IsNullOrWhiteSpace(options.OutputFolder))
                throw new ArgumentException(nameof(options.OutputFolder));

            if (!Directory.Exists(options.OutputFolder))
                Directory.CreateDirectory(options.OutputFolder);
        }
    }
}
