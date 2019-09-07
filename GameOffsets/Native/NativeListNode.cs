using System;
using System.Runtime.InteropServices;

namespace GameOffsets.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NativeListNode
    {
        public long Next;
        public long Prev;
        public long Ptr1_Unused;
        public long Ptr2_Key;
        public int Value;
    }
}