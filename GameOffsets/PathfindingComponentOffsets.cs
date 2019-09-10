using System.Runtime.InteropServices;
using SharpDX;
using GameOffsets.Native;

namespace GameOffsets
{
 
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    public struct PathfindingComponentOffsets
    {
        [FieldOffset(0x28)] public Vector2i ClickToNextPosition;
        [FieldOffset(0x30)] public Vector2i WasInThisPosition;
        [FieldOffset(0x468)] public byte IsMoving;
        [FieldOffset(0x4A8)] public Vector2i WantMoveToPosition;
        [FieldOffset(0x4BC)] public float StayTime;
    }
}