namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct WorldItem
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0028)] public long ItemPtr;

        //AllocationLootId of the Character who can pick this item.
        // 0 means anyone can pick it up
        // This value doesn't change in a permanent allocation system.
        // Haven't tested soft allocation system.
        [FieldOffset(0x0030)] public int LootAllocationId;

        // CanPickUp = TimeLeftForFreeForAll <= 0
        // TimeLeftForFreeForAll = Future_GetTickCount - Current_GetTickCount
        // Future_GetTickCount = DroppedTime + LootAllocationTime;

        // 0x493E0 for permanent allocation ( in milliseconds )
        // 0x00000 for free for all or if that person leaves the map
        [FieldOffset(0x0034)] public uint LootAllocationTime;

        // basically value returned from Kernel32.GetTickCount
        // at the time the item was dropped. Read GetTickCount
        // defination for more details.
        [FieldOffset(0x0038)] public int DroppedTime;

        [FieldOffset(0x0038)] public float Unknown0;
        [FieldOffset(0x0040)] public byte Unknown1;
        [FieldOffset(0x0041)] public byte Unknown2;
    }
}