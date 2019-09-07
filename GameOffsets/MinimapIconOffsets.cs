using GameOffsets.Native;
using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MinimapIconOffsets
    {
        [FieldOffset(0x28)] public long NamePtr;
        [FieldOffset(0x30)] public byte IsVisible;
        [FieldOffset(0x34)] public byte IsHide;
        
    }
}