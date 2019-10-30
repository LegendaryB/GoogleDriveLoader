using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleDriveLoader
{
    class App
    {
        private static CancellationTokenSource _cts;

        static async Task Main()
        {
            var options = AppOptions.Get();

            FolderStructure.Instance
                .SetOutputFolder(options.OutputFolder);

            _cts = new CancellationTokenSource();

            Clipboard.Instance.Listen(_cts.Token);

            Console.Read();
        }
    }
}
