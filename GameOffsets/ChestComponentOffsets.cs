using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ChestComponentOffsets
    {
        [FieldOffset(0x78)] public bool IsOpened;
        [FieldOffset(0x79)] public bool IsLocked;
        [FieldOffset(0xB8)] public bool IsStrongbox;
        [FieldOffset(0x7C)] public readonly byte quality;
        [FieldOffset(0x58)] public long StrongboxData;
    }
}
