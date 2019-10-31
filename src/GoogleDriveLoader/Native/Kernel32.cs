using System.Runtime.InteropServices;

namespace GoogleDriveLoader.Native
{
    internal class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AllocConsole();
    }
}
