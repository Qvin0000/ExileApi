using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ObjectHeaderOffsets
    {
        [FieldOffset(0x0)] public long MainObject;
        [FieldOffset(0x40)] public NativePtrArray ComponentList;

        public override string ToString()
        {
            return $"MainObject: {MainObject} ComponentList:{ComponentList}";
        }
    }
}
