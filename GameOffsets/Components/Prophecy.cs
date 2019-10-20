namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Prophecy
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0020)] public long PropheciesDatPtr; //PropheciesDatStructure
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct PropheciesDatStructure
    {
        [FieldOffset(0x0000)] public long IdString;
        [FieldOffset(0x0008)] public long PredictionText;
        [FieldOffset(0x0010)] public int IdNumber;
        [FieldOffset(0x0014)] public long Name;
        [FieldOffset(0x001C)] public long FlavourText;
        [FieldOffset(0x0024)] public long TotalClientStringKeys;
        [FieldOffset(0x002C)] public long ClientStringKeysPtr;
        [FieldOffset(0x0034)] public long AudioFile;
        [FieldOffset(0x0044)] public long ProphecyChainPtr; //ProphecyChainDatStructure
        [FieldOffset(0x004C)] public int ProphecyChainPosition;
        [FieldOffset(0x0050)] public byte IsEnabled;
        [FieldOffset(0x0051)] public int SealCost;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ProphecyChainDatStructure
    {
        //Variable Names picked from GGPK file.
        [FieldOffset(0x0000)] public long IdString;
        [FieldOffset(0x0008)] public int Unknown1;
        [FieldOffset(0x000C)] public long TotalUnknown2Keys;
        [FieldOffset(0x0014)] public long Unknown2Ptr;
        [FieldOffset(0x001C)] public long TotalUnknown3Keys;
        [FieldOffset(0x0024)] public long Unknown3Ptr;
        [FieldOffset(0x002C)] public int Unknown4;
        [FieldOffset(0x0030)] public int Unknown5;
    }
}