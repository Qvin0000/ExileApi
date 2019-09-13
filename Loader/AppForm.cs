using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SharpDX.Windows;

namespace Loader
{
    public class AppForm : RenderForm
    {
        private readonly ContextMenu contextMenu1;
        public Action FixImguiCapture;
        private readonly NotifyIcon notifyIcon;

        public AppForm()
        {
            SuspendLayout();
            contextMenu1 = new ContextMenu();
            var menuItem1 = new MenuItem();
            var menuItem2 = new MenuItem();
            contextMenu1.MenuItems.AddRange(new[] {menuItem1, menuItem2});

            menuItem2.Index = 0;
            menuItem2.Text = "Bring to Front";

            menuItem1.Index = 1;
            menuItem1.Text = "E&xit";
            notifyIcon = new NotifyIcon();
            notifyIcon.ContextMenu = contextMenu1;
            notifyIcon.Icon = Icon;
            menuItem1.Click += (sender, args) => { Close(); };

            menuItem2.Click += (sender, args) =>
            {
                BringToFront();
                FixImguiCapture?.Invoke();
            };

            Icon = Icon.ExtractAssociatedIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "textures\\icon.ico"));
            notifyIcon.Icon = Icon;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);

            Text = "ExileApi";
            notifyIcon.Text = "ExileApi";
            notifyIcon.Visible = true;
            Size = new Size(1600, 900); //Screen.PrimaryScreen.Bounds.Size;
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            ShowInTaskbar = true;
            BackColor = Color.Black;

            ResumeLayout(false);
            BringToFront();
        }

        protected override void Dispose(bool disposing)
        {
            if (notifyIcon != null)
                notifyIcon.Icon = null;

            notifyIcon?.Dispose();
            contextMenu1?.Dispose();
            base.Dispose(disposing);
        }
    }
}
