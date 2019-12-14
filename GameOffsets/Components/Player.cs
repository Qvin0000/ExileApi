namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;
    using SharpDX;
    using GameOffsets.Native;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Player
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0018)] public NativePtrArray EntitiesAroundPlayerPtr;
        [FieldOffset(0x0058)] public NativeUnicodeText CharactorName;
        //[FieldOffset(0x0078)] public int Unknown0;
        // Get 2 people (including you) in the game, find an item belonging to the other person
        // Open WorldItem component of that item. Find a 4 byte uint that matches from there to here.
        [FieldOffset(0x007C)] public uint LootAllocationId;
        [FieldOffset(0x007C)] public int Experience;
        [FieldOffset(0x0080)] public int Strength;
        [FieldOffset(0x0084)] public int Dexterity;
        [FieldOffset(0x0088)] public int Intelligence;
        [FieldOffset(0x00A8)] public byte Level;
        //[FieldOffset(0x0091)] public byte Unknown2;
        //[FieldOffset(0x0092)] public byte Unknown3;
        [FieldOffset(0x0093)] public byte MinorPantheonSkillId;
        [FieldOffset(0x0094)] public byte MajorPantheonSkillId;
        //[FieldOffset(0x0098)] public int Unknown4;
        //[FieldOffset(0x009C)] public int Unknown5;
        [FieldOffset(0x00F8)] public long HideoutPtr; // Ptr to Hideout.dat row
        [FieldOffset(0x0112)] public PropheciesStruct Prophecies;
        [FieldOffset(0x0138)] public long ServerDataPtr;

        // This WorldPosition is different from position component WorldPosition
        // as this one is updated slowly. This might be part of a teleport detection
        // mechanism.
        [FieldOffset(0x0140)] public Vector3 WorldPosition;
    }

    // Don't need FieldOffset since they are Consecutive.
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct PropheciesStruct
    {

        public byte TotalActive;
        public byte Unknown0;
        // TODO: I wonder if we can create
        // fixed array of const size 7 over here?
        public ProphecyStruct P1;
        public ProphecyStruct P2;
        public ProphecyStruct P3;
        public ProphecyStruct P4;
        public ProphecyStruct P5;
        public ProphecyStruct P6;
        public ProphecyStruct P7;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ProphecyStruct
    {
        [FieldOffset(0x0000)] public short Id;
        [FieldOffset(0x0002)] public byte index;
        // Might be padding or might be something related to the prophecy
        //[FieldOffset(0x0003)] public byte Unknown0;
    }
}
