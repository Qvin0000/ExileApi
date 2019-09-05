using System;
using System.Diagnostics;
using Basic;
using Exile;
using Shared;
using Shared.Interfaces;
using SharpDX;

namespace Plugins
{
    public class GameWindow
    {
        private readonly IntPtr handle;

        public GameWindow(Process process) {
            Process = process;
            handle = process.MainWindowHandle;
            _getWindowRectangle = new TimeCache<RectangleF>(GetWindowRectangleReal, 200);
        }

        public Process Process { get; private set; }
        private CachedValue<RectangleF> _getWindowRectangle;

        public RectangleF GetWindowRectangle() => _getWindowRectangle.Value;
        public RectangleF GetWindowRectangleTimeCache => _getWindowRectangle.Value;

        private System.Drawing.Rectangle _lastValid = System.Drawing.Rectangle.Empty;

        public RectangleF GetWindowRectangleReal() {
            var rectangle = WinApi.GetClientRectangle(handle);
            if (rectangle.Width < 0 && rectangle.Height < 0)
                rectangle = _lastValid;
            else
                _lastValid = rectangle;
            return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public bool IsForeground() => WinApi.IsForegroundWindow(handle);

        public Vector2 ScreenToClient(int x, int y) {
            var point = new Point(x, y);
            WinApi.ScreenToClient(handle, ref point);
            return point;
        }
    }
}