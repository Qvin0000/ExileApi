using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FilesOffsets
    {
        [FieldOffset(0x10)] public long String;

        [FieldOffset(0x18)] public long MoreInformation;
        /*public string ToString(IMemory mem) {
            var size = mem.Read<FileInformation>(MoreInformation);
            if (size.Size < 512)
                return mem.ReadStringU(String.ToInt64(), (int) (size.Size * 2));
            return mem.ReadStringU(String.ToInt64());
        }*/
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FileInformation
    {
        [FieldOffset(0x10)] public NativeStringU String;
        [FieldOffset(0x20)] public long Size;
        [FieldOffset(0x20)] public long Capacity;
        [FieldOffset(0x28)] public int Test1;
        [FieldOffset(0x30)] public int AreaCount;
        [FieldOffset(0x4C)] public int Test2;
    }
}
