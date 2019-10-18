namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Map
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        //[FieldOffset(0x0010)] public long MapInternalPtr;
        [FieldOffset(0x0018)] public byte Tier;
        //// MapSeries.dat enum, GL updating that every league.
        //// Sometime we get 0x00, mark it as unknown.
        //[FieldOffset(0x0019)] public byte Series;
    }

    //[StructLayout(LayoutKind.Explicit, Pack = 1)]
    //public struct MapInternalStruct
    //{
    //    [FieldOffset(0x28)] public long AreaAddress;
    //}
}