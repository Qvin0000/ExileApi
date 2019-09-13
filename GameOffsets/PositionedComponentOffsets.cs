using System.Runtime.InteropServices;
using SharpDX;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct PositionedComponentOffsets
    {
        [FieldOffset(0x8)] public long OwnerAddress;
        [FieldOffset(0xEC)] public Vector2 GridPosition;
        [FieldOffset(0xEC)] public int GridX;
        [FieldOffset(0xF0)] public int GridY;
        [FieldOffset(0x118)] public Vector2 WorldPosition;
        [FieldOffset(0x118)] public float WorldX;
        [FieldOffset(0x11C)] public float WorldY;
        [FieldOffset(0x64)] public int Size;
        [FieldOffset(0xF0)] public float Rotation;

        //[FieldOffset(0x138)] public byte  Reaction;
        [FieldOffset(0x58)] public byte Reaction;
    }
}
