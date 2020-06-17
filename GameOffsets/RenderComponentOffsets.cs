using System.Runtime.InteropServices;
using GameOffsets.Native;
using SharpDX;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct RenderComponentOffsets
    {
        [FieldOffset(0x80)] public Vector3 Pos;
		[FieldOffset(0x8C)] public Vector3 Bounds;
		[FieldOffset(0xA0)] public NativeStringU Name;
        [FieldOffset(0xC0)] public Vector3 Rotation;
		[FieldOffset(0xE0)] public float Height;
	}
}
