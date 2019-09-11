using System;
using System.Diagnostics;
using ExileCore.Shared;
using ExileCore.Shared.Cache;
using SharpDX;
using Rectangle = System.Drawing.Rectangle;

namespace ExileCore
{
    public class GameWindow
    {
        private readonly IntPtr handle;
        private readonly CachedValue<RectangleF> _getWindowRectangle;
        private Rectangle _lastValid = Rectangle.Empty;

        public GameWindow(Process process)
        {
            Process = process;
            handle = process.MainWindowHandle;
            _getWindowRectangle = new TimeCache<RectangleF>(GetWindowRectangleReal, 200);
        }

        public Process Process { get; }
        public RectangleF GetWindowRectangleTimeCache => _getWindowRectangle.Value;

        public RectangleF GetWindowRectangle()
        {
            return _getWindowRectangle.Value;
        }

        public RectangleF GetWindowRectangleReal()
        {
            var rectangle = WinApi.GetClientRectangle(handle);

            if (rectangle.Width < 0 && rectangle.Height < 0)
                rectangle = _lastValid;
            else
                _lastValid = rectangle;

            return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public bool IsForeground()
        {
            return WinApi.IsForegroundWindow(handle);
        }

        public Vector2 ScreenToClient(int x, int y)
        {
            var point = new Point(x, y);
            WinApi.ScreenToClient(handle, ref point);
            return point;
        }
    }
}
