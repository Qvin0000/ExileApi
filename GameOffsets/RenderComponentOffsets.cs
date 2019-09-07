using System.Runtime.InteropServices;
using GameOffsets.Native;
using SharpDX;

namespace GameOffsets
{

    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    public struct RenderComponentOffsets
    {
	[FieldOffset((int)Offsets.RenderComponentOffsetsPos)] public Vector3 Pos;
	[FieldOffset((int)Offsets.RenderComponentOffsetsBounds)] public Vector3 Bounds;
	[FieldOffset((int)Offsets.RenderComponentOffsetsName)] public NativeStringU Name;
	[FieldOffset((int)Offsets.RenderComponentOffsetsRotation)] public Vector3 Rotation;
	[FieldOffset((int)Offsets.RenderComponentOffsetsHeight)] public float Height;      
    }
}