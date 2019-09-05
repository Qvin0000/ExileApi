using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Exile;
using ImGuiNET;
using Shared;
using SharpDX.Windows;

namespace Loader
{
    public class AppForm : RenderForm
    {
        private NotifyIcon notifyIcon;
        private ContextMenu contextMenu1;


        public AppForm() {
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
                ImGui.CaptureMouseFromApp();
                ImGui.CaptureKeyboardFromApp();
            };
            Icon = Icon.ExtractAssociatedIcon("textures\\poehud.ico");
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

        protected override void Dispose(bool disposing) {
            if (notifyIcon != null)
            {
                notifyIcon.Icon = null;
            }
            notifyIcon?.Dispose();
            contextMenu1?.Dispose();
            base.Dispose(disposing);
        }
    }
}