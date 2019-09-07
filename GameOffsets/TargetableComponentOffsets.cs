using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    public struct TargetableComponentOffsets
    {
		[FieldOffset(0x30)]
        public bool isTargetable;
        [FieldOffset(0x32)]
        public bool isTargeted;
    }
}