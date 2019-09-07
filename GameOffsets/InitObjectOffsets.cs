using System;
using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    public struct InitObjectOffsets
    {
        public long vTable;
        public long ParentObjectPtr;
    }
}