using System.Runtime.InteropServices;
using GameOffsets.Native;
using SharpDX;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct RenderComponentOffsets
    {
        [FieldOffset(0x90)] public Vector3 Pos;
        [FieldOffset(0x9C)] public Vector3 Bounds;
        [FieldOffset(0xB0)] public NativeStringU Name;
        [FieldOffset(0xD0)] public Vector3 Rotation;
        [FieldOffset(0xF8)] public float Height;
	}
}
