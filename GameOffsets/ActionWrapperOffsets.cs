using System;
using System.Runtime.InteropServices;
using SharpDX;

namespace GameOffsets
{
 
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ActionWrapperOffsets
    {

        [FieldOffset(0x60)] public Vector2 Destination;
        [FieldOffset(0x38)] public long Target;
        [FieldOffset(0x18)] public long Skill;

    }
}
