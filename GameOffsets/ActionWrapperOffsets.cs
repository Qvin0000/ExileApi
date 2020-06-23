using System.Runtime.InteropServices;
using SharpDX;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ActionWrapperOffsets
    {
        [FieldOffset(0x78)] public Vector2 Destination;
        [FieldOffset(0x70)] public long Target;
        [FieldOffset(0x48)] public long Skill;
    }
}
