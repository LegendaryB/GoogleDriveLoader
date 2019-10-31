using System;

namespace GoogleDriveLoader
{
    internal static class ConsoleOutput
    {
        internal static void WriteLine(string message)
        {
            Console.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}
