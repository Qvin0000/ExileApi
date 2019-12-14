namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Targetable
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0028)] public long UnknownPtr0;
        [FieldOffset(0x0030)] public byte IsTargetable;
        [FieldOffset(0x0031)] public byte IsHighlightable;
        [FieldOffset(0x0032)] public byte IsTargetted;
        [FieldOffset(0x0033)] public byte UnknownBool0;
        [FieldOffset(0x0034)] public byte UnknownBool1;
        [FieldOffset(0x0035)] public byte UnknownBool2;
        [FieldOffset(0x0036)] public byte UnknownBool3;
        [FieldOffset(0x0037)] public byte UnknownBool4;
        [FieldOffset(0x0038)] public int UnknownInt0;
        [FieldOffset(0x003C)] public int UnknownInt1;
        [FieldOffset(0x0040)] public int UnknownInt2;
        [FieldOffset(0x0044)] public int UnknownInt3;
    }
}