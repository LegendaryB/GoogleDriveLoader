using System;
using System.Diagnostics;
using System.Threading;

namespace GoogleDriveLoader
{
    static class Program
    {
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        static void Main()
        {
            //Console.CancelKeyPress += Console_CancelKeyPress;

            WindowHandler.CreateWindow(_cts.Token);

            Console.ReadLine();
        }
    }
}
