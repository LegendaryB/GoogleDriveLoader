using GoogleDriveLoader.Native;

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace GoogleDriveLoader
{
    public partial class ClipboardNotificationWindow : Form
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly CancellationTokenSource _cts;

        private string previousClipboardContent;

        public ClipboardNotificationWindow(CancellationTokenSource cts)
        {
            InitializeComponent();

            Size = Size.Empty;
            ShowInTaskbar = false;
            Visible = false;

            _cts = cts;

            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipTitle = "Google Drive link captured"
            };

            var exitMenuItem = new ToolStripMenuItem("Exit", null, OnExitTrayMenuItemClicked);

            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add(exitMenuItem);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            User32.SetParent(Handle, (IntPtr)User32.HwndMessage);
            User32.AddClipboardFormatListener(Handle);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg != User32.WM_CLIPBOARDUPDATE)
                return;

            if (!Clipboard.ContainsText())
                return;

            var data = Clipboard.GetText();

            if (data != previousClipboardContent && data.Contains("drive.google.com/drive/folders/"))
            {
                MediaQueue.Enqueue(data);

                _notifyIcon.BalloonTipText = data;
                _notifyIcon.ShowBalloonTip(1000);

                previousClipboardContent = data;
            }
        }

        private void OnExitTrayMenuItemClicked(object sender, EventArgs e)
        {
            User32.RemoveClipboardFormatListener(Handle);
            _cts.Cancel();
            Application.Exit();
        }
    }
}
