using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ChestComponentOffsets
    {
        [FieldOffset(0x178)] public bool IsOpened;
        [FieldOffset(0x179)] public bool IsLocked;
        [FieldOffset(0x1B8)] public bool IsStrongbox;
        [FieldOffset(0x17C)] public readonly byte quality;
        [FieldOffset(0x158)] public long StrongboxData;
    }
}
