using System.Runtime.InteropServices;
using SharpDX;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct CameraOffsets
    {
        [FieldOffset(0x4)] public int Width;
        [FieldOffset(0x8)] public int Height;
        [FieldOffset(0x1C8)] public float ZFar;
        [FieldOffset(0xD4)] public Vector3 Position;
        [FieldOffset(0x5C)] public Matrix MatrixBytes;
    }
}
