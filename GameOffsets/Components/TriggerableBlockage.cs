namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct TriggerableBlockage
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0030)] public byte IsClosed;
    }
}