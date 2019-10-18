namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MinimapIcon
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        //// Pointer to Data/MinimapIcons.dat row
        [FieldOffset(0x0028)] public long MinimapIconInternalPtr;
        [FieldOffset(0x0034)] public byte IsHidden;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MinimapIconInternalStruct
    {
        [FieldOffset(0x0000)] public long IconNamePtr;
        [FieldOffset(0x0008)] public int MinimapIconRadius;
        [FieldOffset(0x000C)] public int LargemapIconRadius;
        //// 0x10, 0x11, 0x12 are bool flags, even PyPoe doesn't know that they are. Looks useless
        [FieldOffset(0x0013)] public int MinimapIconPointerMaxDistance;
        //// 0x17 is an int, don't know what it is, even PyPoe doesn't know that.
    }
}