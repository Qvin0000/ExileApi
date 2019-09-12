using System;
using System.Runtime.InteropServices;

namespace ExileCore.Shared.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ClientID
    {
        public IntPtr UniqueProcess;
        public IntPtr UniqueThread;
    }
}
