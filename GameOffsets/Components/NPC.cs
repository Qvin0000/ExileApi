namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct NPC
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0018)] public long NpcInternalPtr; //NPCInternalStruct
        [FieldOffset(0x0020)] public bool IsMapLabelVisible;
        [FieldOffset(0x0048)] public long IconOverheadPtr;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct NPCInternalStruct
    {
        [FieldOffset(0x0018)] public long NpcDatPtr; //NPCDatStruct
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct NPCDatStruct
    {
        [FieldOffset(0x0000)] public long IdStringPtr;
        [FieldOffset(0x0008)] public long NamePtr;
        [FieldOffset(0x0010)] public long MetadataPtr;
        [FieldOffset(0x0018)] public int Unknown0;
        // IsNpcMaster if greater than 0.
        [FieldOffset(0x0024)] public long NpcMasterPtr;
        [FieldOffset(0x002c)] public long ShortNamePtr;
        // -1, 0 means no specific ACT (Hideout, Maps etc)
        [FieldOffset(0x0034)] public int Act;
    }
}