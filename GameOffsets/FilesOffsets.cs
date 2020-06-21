using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FilesOffsets
    {
        [FieldOffset(0x8)] public long ListPtr;
        [FieldOffset(0x18)] public long MoreInformation;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FileNode
    {
        public long Next;
        public long Prev;
        public long Key;
        public long Value;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FileInformation
    {
        [FieldOffset(0x08)] public NativeStringU String;
        [FieldOffset(0x18)] public long Size;
        [FieldOffset(0x20)] public long Capacity;
        [FieldOffset(0x30)] public int Test1;
        [FieldOffset(0x38)] public int AreaCount;
        [FieldOffset(0x40)] public int Test2;
    }
}
