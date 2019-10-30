using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GoogleDriveLoader
{
    public partial class ClipboardNotificationWindow : Form
    {
        private static readonly HandleRef HwndMessage = new HandleRef(null, new IntPtr(-3));

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// Places the given window in the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Removes the given window from the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Sent when the contents of the clipboard have changed.
        /// </summary>
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        private string previousClipboardContent;

        public ClipboardNotificationWindow(CancellationToken token)
        {
            InitializeComponent();

            Size = Size.Empty;
            ShowInTaskbar = false;
            Visible = false;

            token.Register(() =>
            {
                var test = RemoveClipboardFormatListener(Handle);
                Application.ExitThread();
            });
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            SetParent(Handle, (IntPtr)HwndMessage);
            AddClipboardFormatListener(Handle);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg != WM_CLIPBOARDUPDATE)
                return;

            if (!Clipboard.ContainsText())
                return;

            var data = Clipboard.GetText();

            if (data != previousClipboardContent)
            {
                // todo: event
                Console.WriteLine(data);
                previousClipboardContent = data;
            }
        }
    }
}
