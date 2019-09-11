using System;

namespace ExileCore.Shared.Interfaces
{
    public interface INativePtrArray
    {
        IntPtr First { get; }
        IntPtr Last { get; }
        IntPtr End { get; }
        string ToString();
    }
}
