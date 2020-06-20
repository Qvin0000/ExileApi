using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FilesOffsets
    {
        [FieldOffset(0x8)] public long ListPtr;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FileNode
    {
        public long Next;
        public long Prev;
        public long Key;
        public long Value;
    }
}
