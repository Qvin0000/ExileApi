namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Portal
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        // Got the value from following file.
        // Metadata/MiscellaneousObjects/MultiplexPortal.ot
        // 0: Uninitialised
        // 1: Player
        // 2: Town
        // 3: Map
        // 4: Map Return
        [FieldOffset(0x0020)] public byte Type;
        [FieldOffset(0x0038)] public long WorldAreaDatRowPtr;
    }
}