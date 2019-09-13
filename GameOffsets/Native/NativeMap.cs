using System.Runtime.InteropServices;

namespace GameOffsets.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NativeMap
    {
        public override string ToString()
        {
            return string.Format("Head: 0x{0}, Size: 0x{1}", Head.ToString("X"), Size);
        }

        public readonly long Head;
        public readonly uint Size;
    }
}
