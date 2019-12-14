using System.Runtime.InteropServices;
using SharpDX;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct PositionedComponentOffsets
    {
        [FieldOffset(0x8)] public long OwnerAddress;
        [FieldOffset(0xE0)] public Vector2 GridPosition;
        [FieldOffset(0xE0)] public int GridX;
        [FieldOffset(0xE4)] public int GridY;
        [FieldOffset(0x110)] public Vector2 WorldPosition;
        [FieldOffset(0x110)] public float WorldX;
        [FieldOffset(0x114)] public float WorldY;
        [FieldOffset(0x64)] public int Size;
        [FieldOffset(0xE8)] public float Rotation;

        //[FieldOffset(0x138)] public byte  Reaction;
        [FieldOffset(0x58)] public byte Reaction;
    }
}
