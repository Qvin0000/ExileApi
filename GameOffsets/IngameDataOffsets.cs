using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct IngameDataOffsets
    {
        [FieldOffset(0x60)] public long CurrentArea;
        [FieldOffset(0x78)] public byte CurrentAreaLevel;
        [FieldOffset(0xDC)] public uint CurrentAreaHash;
		[FieldOffset(0xF0)] public NativePtrArray MapStats;
        [FieldOffset(0x11C)] public long LabDataPtr;
        [FieldOffset(0x480)] public long LocalPlayer;
        [FieldOffset(0x508)] public long EntityList;
        [FieldOffset(0x510)] public long EntitiesCount;
    }
}
