using System;
using System.Runtime.InteropServices;

namespace GameOffsets.Native
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct NativeHashNode
    {
        public override string ToString()
        {
            return $"NativeHashNode";
        }

[FieldOffset(0x0)]
        public readonly long Previous;
		[FieldOffset(0x8)]
		  public readonly long Root;
		  [FieldOffset(0x10)]
		    public readonly long Next;
			[FieldOffset(0x19)]
			public readonly byte IsNull;
			[FieldOffset(0x20)]
			public int Key;
			[FieldOffset(0x28)]
			public long Value1;
			
		
    }
}