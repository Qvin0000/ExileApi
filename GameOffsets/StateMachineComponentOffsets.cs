using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct StateMachineComponentOffsets
    {
        [FieldOffset(0x20)] public long StatesPtr;
        [FieldOffset(0x38)] public NativePtrArray StatesValues;
    }
}