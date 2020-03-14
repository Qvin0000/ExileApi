using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct NormalInventoryItemOffsets
    {
        [FieldOffset(0x14)] public int ToolTip;
		[FieldOffset(0x388)] public long Item;
		[FieldOffset(0x390)] public int InventPosX;
		[FieldOffset(0x394)] public int InventPosY;
		[FieldOffset(0x398)] public int Width;
		[FieldOffset(0x39c)] public int Height;
	}
}
