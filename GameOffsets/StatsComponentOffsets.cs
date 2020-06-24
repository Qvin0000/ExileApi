using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct StatsComponentOffsets
    {
        [FieldOffset(0x8)] public long Owner;
        [FieldOffset(0x20)] public long SubStatsPtr;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct SubStatsComponentOffsets
    {
        [FieldOffset(0xF0)] public NativePtrArray Stats;
    }
}
