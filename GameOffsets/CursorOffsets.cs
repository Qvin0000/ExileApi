using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct CursorOffsets
    {
        public  const int OffsetBuffers = 0x6EC;

        [FieldOffset(0x0)] public int vTable;
        [FieldOffset(0x238)] public int Action;
        [FieldOffset(0x24C)] public int Clicks;
        [FieldOffset(0x2A0)] public NativeStringU ActionString;
        
    }
}