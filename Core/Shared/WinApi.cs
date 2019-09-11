using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ProcessMemoryUtilities.Memory;
using Point = SharpDX.Point;

namespace ExileCore.Shared
{
    public static class WinApi
    {
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_RESTORE = 9;
        private const short SWP_NOMOVE = 0X2;
        private const short SWP_NOSIZE = 1;
        private const short SWP_NOZORDER = 0X4;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int GWL_EXSTYLE = -20;
        private const int GWLP_HINSTANCE = -6;
        private const int GWLP_ID = -12;
        private const int GWL_STYLE = -16;
        private const int GWLP_USERDATA = -21;
        private const int GWLP_WNDPROC = -4;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_TOPMOST = 0x00000008;
        private const int WS_VISIBLE = 0x10000000;
        private const int LWA_ALPHA = 0x2;
        private const int LWA_COLORKEY = 0x1;

        public static void EnableTransparent(IntPtr handle)
        {
            SetWindowLong(handle, GWL_STYLE, new IntPtr(WS_VISIBLE));
            SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(WS_EX_LAYERED));
            var margins = Margins.FromRectangle(new Rectangle(-1, -1, -1, -1)); // Margins.FromRectangle(size);
            DwmExtendFrameIntoClientArea(handle, ref margins);
        }

        public static void SetTransparent(IntPtr handle)
        {
            SetWindowLong(handle, GWL_STYLE, new IntPtr(WS_VISIBLE));
            SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST));
        }

        public static void SetNoTransparent(IntPtr handle)
        {
            SetWindowLong(handle, GWL_STYLE, new IntPtr(WS_VISIBLE));
            SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(WS_EX_LAYERED | WS_EX_TOPMOST));
        }

        public static void EnableTransparentByColorRef(IntPtr handle, Rectangle size, int color)
        {
            var windowLong = GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_LAYERED;
            SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(windowLong));
            SetLayeredWindowAttributes(handle, (uint) color, 100, LWA_ALPHA | LWA_COLORKEY);
            var margins = Margins.FromRectangle(size);
            DwmExtendFrameIntoClientArea(handle, ref margins);
        }

        public static IntPtr OpenProcess(Process process, ProcessAccessFlags flags)
        {
            return OpenProcess(flags, false, process.Id);
        }

        public static bool IsForegroundWindow(IntPtr handle)
        {
            return GetForegroundWindow() == handle;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool AllowSetForegroundWindow(uint dwProcessId);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        public static Rectangle GetClientRectangle(IntPtr handle)
        {
            GetClientRect(handle, out var rect);
            ClientToScreen(handle, out var point);
            return rect.ToRectangle(point);
        }

        public static int MakeCOLORREF(byte r, byte g, byte b)
        {
            return (int) (r | ((uint) g << 8) | ((uint) b << 16));
        }

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, out Point lpPoint);

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("dwmapi.dll")]
        private static extern IntPtr DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        [DllImport("USER32.dll")]
        public static extern short GetKeyState(Keys vKey);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        public static Point GetCursorPosition(IntPtr hWnd)
        {
            GetCursorPos(out var lpPoint);

            //bool success = User32.GetCursorPos(out lpPoint);
            // if (!success)

            ScreenToClient(hWnd, ref lpPoint);
            return lpPoint;
        }

        public static Point GetCursorPositionPoint()
        {
            GetCursorPos(out var lpPoint);
            return lpPoint;
        }

        public static bool ReadProcessMemory(IntPtr handle, IntPtr baseAddress, byte[] buffer)
        {
            return ReadProcessMemory(handle, baseAddress, buffer, buffer.Length, out _);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hWnd, IntPtr baseAddr, byte[] buffer, int size, out IntPtr bytesRead);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct Margins
        {
            private int left, right, top, bottom;

            public static Margins FromRectangle(Rectangle rectangle)
            {
                var margins = new Margins {left = rectangle.Left, right = rectangle.Right, top = rectangle.Top, bottom = rectangle.Bottom};
                return margins;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            private readonly int left, top, right, bottom;

            public Rectangle ToRectangle(Point point)
            {
                return new Rectangle(point.X, point.Y, right - left, bottom - top);
            }
        }
    }
}
