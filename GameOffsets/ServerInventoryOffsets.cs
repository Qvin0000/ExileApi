using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ServerInventoryOffsets
    {
        [FieldOffset(0x1)] public byte InventSlot;
        [FieldOffset(0x48)] public long InventorySlotItemsPtr;
    }
}
