using System.Runtime.InteropServices;
using SharpDX;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct CameraOffsets
    {
        [FieldOffset(0x4)] public int Width;
        [FieldOffset(0x8)] public int Height;
        [FieldOffset(0x210)] public float ZFar;
        [FieldOffset(0xD4)] public Vector3 Position;

        //First value is changing when we change the screen size (ratio)
        //4 bytes before the matrix doesn't change
        [FieldOffset(0xA4)] public Matrix MatrixBytes;
    }
}
