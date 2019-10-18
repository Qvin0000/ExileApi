namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Charges
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0010)] public long ChargesInternalPtr;
        [FieldOffset(0x0018)] public int CurrentCharges;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ChargesInternalStruct
    {
        [FieldOffset(0x0010)] public int MaxCharges;
        [FieldOffset(0x0014)] public int PerCharges;
    }
}