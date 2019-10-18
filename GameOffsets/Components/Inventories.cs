namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Inventories
    {
        public const int StartingPointOffset = 0x0040;
        public const int DataStructureLength = 0x0060;
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(StartingPointOffset  + 0 * DataStructureLength )] public long LeftHandPtr;
        [FieldOffset(StartingPointOffset  + 1 * DataStructureLength )] public long RightHandPtr;
        [FieldOffset(StartingPointOffset  + 2 * DataStructureLength )] public long ChestPtr;
        [FieldOffset(StartingPointOffset  + 3 * DataStructureLength )] public long HelmPtr;
        [FieldOffset(StartingPointOffset  + 4 * DataStructureLength )] public long GlovesPtr;
        [FieldOffset(StartingPointOffset  + 5 * DataStructureLength )] public long BootsPtr;
        [FieldOffset(StartingPointOffset  + 6 * DataStructureLength )] public long UnknownPtr;
        [FieldOffset(StartingPointOffset  + 7 * DataStructureLength )] public long LeftRingPtr;
        [FieldOffset(StartingPointOffset  + 8 * DataStructureLength )] public long RightRingPtr;
        [FieldOffset(StartingPointOffset  + 9 * DataStructureLength )] public long BeltPtr;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct InventoryInternalStruct
    {
        [FieldOffset(0x0000)] public long ItemName;
        [FieldOffset(0x0008)] public long ItemTexture;
        [FieldOffset(0x0010)] public long ItemModel;
    }
}