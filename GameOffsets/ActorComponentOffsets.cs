using System;
using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ActorComponentOffsets
    {

        [FieldOffset(0x98)] public int ActionId;
        [FieldOffset(0x438)] public NativePtrArray HasMinionArray;
        [FieldOffset(0x440)] public NativePtrArray DeployedObjectArray;
        [FieldOffset(0x3D8)] public NativePtrArray ActorSkillsArray;
        [FieldOffset(0x408)] public NativePtrArray ActorVaalSkills;

    }
}
