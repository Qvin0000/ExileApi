using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct TargetableComponentOffsets
    {
        [FieldOffset(0x48)] public bool isTargetable;
        [FieldOffset(0x49)] public bool isHighlightable;
        [FieldOffset(0x4A)] public bool isTargeted;
    }
}
