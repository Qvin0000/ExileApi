namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;
    using GameOffsets.Native;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Actor
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        //[FieldOffset(0x0098)] public long ActionPtr;
        [FieldOffset(0x00F8)] public short ActionId;
        // [FieldOffset(0x00FA)] public short TotalActionCounterA;
        // [FieldOffset(0x00FC)] public int TotalActionCounterB;
        // only works for channeling skills
        // [FieldOffset(0x0100)] public float TotalTimeInAction;
        // some unknown timer whos value keep resetting to zero.
        // [FieldOffset(0x0104)] public float UnknownTimer;
        [FieldOffset(0x0120)] public int AnimationId;
        // Use the one inside the ActionPtr struct (i.e. ActionWrapperOffsets).
        // That one works for all kind of skills.
        // [FieldOffset(0x0128)] public Vector2 SkillDestination;
        [FieldOffset(0x03E8)] public NativePtrArray ActorSkillsArray;
        // Broken Offset, remove comment on fixup.
        // [FieldOffset(0x0418)] public NativePtrArray ActorVaalSkills;
        // [FieldOffset(0x0438)] public NativePtrArray HasMinionArray;
        [FieldOffset(0x0450)] public NativePtrArray DeployedObjectArray;
    }
}