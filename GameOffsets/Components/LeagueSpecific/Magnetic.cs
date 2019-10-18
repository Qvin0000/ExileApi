namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Magnetic
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0030)] public int Force;
    }
}