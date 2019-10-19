namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Shrine
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0020)] public int EntityIdWhoLastPickedTheShrine;
        [FieldOffset(0x0024)] public byte IsUsed;
        [FieldOffset(0x0030)] public long ShrineDatRowPtr; //ShrineDatRowStruct
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ShrineDatRowStruct
    {
        [FieldOffset(0x0000)] public long IdString;
         //0 means it's not going to come back again.
        [FieldOffset(0x0008)] public int TimeoutInSeconds;
        [FieldOffset(0x000C)] public long DisplayName;
        [FieldOffset(0x0014)] public bool IsRegain;
        [FieldOffset(0x001D)] public bool PlayerShrineBuffDatRowPtr; //ShrineBuffDatStruct
        [FieldOffset(0x0025)] public int Unknown0;
        [FieldOffset(0x0029)] public int Unknown1;
        [FieldOffset(0x002D)] public long Description;
        [FieldOffset(0x0035)] public long MonsterShrineBuffDatRowPtr; //ShrineBuffDatStruct
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ShrineBuffDatStruct
    {
        [FieldOffset(0x0000)] public long IdString;
        [FieldOffset(0x0008)] public long BuffStatTotalKeys;
        [FieldOffset(0x0010)] public long BuffStatValuesPtr; //Ids (int) for BuffStats.dat
        [FieldOffset(0x0020)] public long BuffDefinationRowPtr; //BuffDefinations.DAT file Row
    }
}