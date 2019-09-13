using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MapElement
    {
        [FieldOffset(0x324 + ElementOffsets.OffsetBuffers)]
        public long LargeMap;

        [FieldOffset(0x32C + ElementOffsets.OffsetBuffers)]
        public long SmallMinMap;

        [FieldOffset(0x344 + ElementOffsets.OffsetBuffers)]
        public long OrangeWords;

        [FieldOffset(0x37C + ElementOffsets.OffsetBuffers)]
        public long BlueWords;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct LargeSmallMapElement
    {
        [FieldOffset(0x2B4 + ElementOffsets.OffsetBuffers)]
        public float LargeMapShiftX;

        [FieldOffset(0x2B8 + ElementOffsets.OffsetBuffers)]
        public float LargeMapShiftY;

        [FieldOffset(0x2F8 + ElementOffsets.OffsetBuffers)]
        public float LargeMapZoom;
    }
}
