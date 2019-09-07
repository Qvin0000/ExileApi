using System;
using System.Runtime.InteropServices;
namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct InventoryOffsets
    {


        [FieldOffset(0x228)] public long HoverItem;
        [FieldOffset(0x230)] public int XFake;
        [FieldOffset(0x234)] public int YFake;
        [FieldOffset(0x238)] public int XReal;
        [FieldOffset(0x23C)] public int YReal;
        [FieldOffset(0x248)] public int CursorInInventory;
        [FieldOffset(0x3B8)] public long ItemCount;
        [FieldOffset(0x4A0)] public int TotalBoxesInInventoryRow;

    }
    
}