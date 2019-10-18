namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct SampleComponentOffset
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
    }
}