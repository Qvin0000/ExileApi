using System;
using System.Runtime.InteropServices;

namespace GameOffsets
{
   [StructLayout(LayoutKind.Explicit,Pack = 1)]
        public struct EntityListOffsets
        {
            [FieldOffset(0x0)] public long FirstAddr;
            [FieldOffset(0x10)] public long SecondAddr;
            [FieldOffset(0x28)] public long Entity;

        }
    
}