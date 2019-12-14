using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct PathEntityOffsets
    {
        [FieldOffset(0x10)] public StringPtr Path;
        [FieldOffset(0x20)] public long Length;

        /*public  string ToString(IMemory mem) { 
            return mem.ReadStringU(Path.Ptr,(int) Length * 2); 
        } */
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct StringPtr
        {
            [FieldOffset(0x0)] public long Ptr;
        }
    }
}
