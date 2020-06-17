using System.Runtime.InteropServices;
using SharpDX;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct PositionedComponentOffsets
    {
		[FieldOffset(8)] public long OwnerAddress;
		[FieldOffset(0x158)] public byte Reaction;
		[FieldOffset(0x164)] public int Size;
		[FieldOffset(0x1AC)] public Vector2 PrevPosition;
		[FieldOffset(0x1C4)] public Vector2 RelativeCoord;
		[FieldOffset(0x1E8)] public Vector2 GridPosition;
		[FieldOffset(0x1E8)] public int GridX;
		[FieldOffset(0x1EC)] public int GridY;
		[FieldOffset(0x1F0)] public float Rotation;
		[FieldOffset(0x218)] public Vector2 WorldPosition;
		[FieldOffset(0x218)] public float WorldX;
		[FieldOffset(0x21C)] public float WorldY;
	}
}
