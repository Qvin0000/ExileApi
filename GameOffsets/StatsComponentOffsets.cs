using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct StatsComponentOffsets
    {
        [FieldOffset(0x8)] public long Owner;
        [FieldOffset(0x118)] public NativePtrArray Stats;
    }
}
