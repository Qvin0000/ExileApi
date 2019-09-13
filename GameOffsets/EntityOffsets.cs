using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct EntityOffsets
    {
        [FieldOffset(0x8)] public ObjectHeaderOffsets Head;
        [FieldOffset(0x10)] public long ComponentList;

        // [FieldOffset(0x40)] public uint Id;
        //  [FieldOffset(0x58)] public uint InventoryId;
        public override string ToString()
        {
            return $"Head: {Head} ComponentList:{ComponentList}";
        }
    }
}
