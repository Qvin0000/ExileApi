using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MapElement
    {
        [FieldOffset(0x230 + ElementOffsets.OffsetBuffers)]
        public long LargeMap;

        [FieldOffset(0x238 + ElementOffsets.OffsetBuffers)]
        public long SmallMinMap;

        [FieldOffset(0x250 + ElementOffsets.OffsetBuffers)]
        public long OrangeWords;

        [FieldOffset(0x2B8 + ElementOffsets.OffsetBuffers)]
        public long BlueWords;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct LargeSmallMapElement
    {
        [FieldOffset(0x1C0 + ElementOffsets.OffsetBuffers)]
        public float LargeMapShiftX;

        [FieldOffset(0x1C4 + ElementOffsets.OffsetBuffers)]
        public float LargeMapShiftY;

        [FieldOffset(0x204 + ElementOffsets.OffsetBuffers)]
        public float LargeMapZoom;
    }
}
