using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ActorComponentOffsets
    {
        [FieldOffset(0x98)] public long ActionPtr;
        [FieldOffset(0xF8)] public short ActionId;

        // [FieldOffset(0xFA)] public short TotalActionCounterA;
        // [FieldOffset(0xFC)] public int TotalActionCounterB;
        // only works for channeling skills
        // [FieldOffset(0x100)] public float TotalTimeInAction;
        // some unknown timer whos value keep resetting to zero.
        // [FieldOffset(0x104)] public float UnknownTimer;
        [FieldOffset(0x120)] public int AnimationId;

        // Use the one inside the ActionPtr struct (i.e. ActionWrapperOffsets).
        // That one works for all kind of skills.
        // [FieldOffset(0x128)] public Vector2 SkillDestination;
        [FieldOffset(0x3E8)] public NativePtrArray ActorSkillsArray;

        // Broken Offset, remove comment on fixup.
        // [FieldOffset(0x418)] public NativePtrArray ActorVaalSkills;
        // [FieldOffset(0x438)] public NativePtrArray HasMinionArray;

        [FieldOffset(0x450)] public NativePtrArray DeployedObjectArray;
    }
}
