using System.Runtime.InteropServices;

namespace GameOffsets
{
   
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct DiagnosticElementArrayOffsets
        {
                [FieldOffset(0x0), MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
                public float[] Values;
                [FieldOffset(0x13C)] public float CurrValue;
        }
}