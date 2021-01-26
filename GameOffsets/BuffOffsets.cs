﻿using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct BuffOffsets
    {
        [FieldOffset(0x8)] public long Name;
        [FieldOffset(0x10)] public byte IsInvisible;
        [FieldOffset(0x11)] public byte IsRemovable;
        [FieldOffset(0x10)] public float MaxTime;
        [FieldOffset(0x14)] public float Timer;
		[FieldOffset(0x30)] public byte Charges;
	}
}
