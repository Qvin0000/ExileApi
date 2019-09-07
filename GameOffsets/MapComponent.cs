using System;
using System.Runtime.InteropServices;
namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MapComponentBase
    {

        [FieldOffset(0x10)] public long Base;

    }
	 [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MapComponentInner
    {

        [FieldOffset(0x28)] public long Area;
        [FieldOffset(0x7B)] public byte Tier;
        [FieldOffset(0x9c)] public int MapSeries;

    }
    
}