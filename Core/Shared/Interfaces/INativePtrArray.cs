using System;

namespace Shared.Interfaces
{
    public interface INativePtrArray
    {
        IntPtr First { get; }
        IntPtr Last { get; }
        IntPtr End { get; }
        string ToString();
    }
}