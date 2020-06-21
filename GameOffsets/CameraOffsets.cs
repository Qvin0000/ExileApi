using System.Runtime.InteropServices;
using SharpDX;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct CameraOffsets
    {
        [FieldOffset(0x8)] public int Width;
        [FieldOffset(0xC)] public int Height;
		//First value is changing when we change the screen size (ratio)
		//4 bytes before the matrix doesn't change
		[FieldOffset(0xA8)] public Matrix MatrixBytes;
		[FieldOffset(0x120)] public Vector3 Position;
		[FieldOffset(0x214)] public float ZFar;

    }
}
