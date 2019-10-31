using System;
using System.Runtime.InteropServices;

namespace GoogleDriveLoader.Native
{
    internal static class User32
    {
        internal static readonly HandleRef HwndMessage = new HandleRef(null, new IntPtr(-3));
        internal const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll")]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
    }
}
