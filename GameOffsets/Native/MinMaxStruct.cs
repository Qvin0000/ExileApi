using System.Runtime.InteropServices;

namespace GameOffsets.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MinMaxStruct
    {
        public int Min;
        public int Max;
    }
}
