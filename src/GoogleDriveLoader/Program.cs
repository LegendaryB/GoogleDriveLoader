using System;
using System.Threading;

namespace GoogleDriveLoader
{
    static class Program
    {
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        static void Main()
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            var window = WindowHandler.CreateWindow(_cts.Token);

            Console.ReadLine();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _cts.Cancel();
        }
    }
}
