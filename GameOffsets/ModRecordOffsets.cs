using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ModsRecordOffsets
    {
        [FieldOffset(0x0)] public NativeStringU Key;
        [FieldOffset(0x8)] public long Unknown8;
		[FieldOffset(0x14)] public long TypeName;
		[FieldOffset(0x1C)] public int MinLevel;
        [FieldOffset(0x28)] public long StatNames1;
        [FieldOffset(0x38)] public long StatNames2;
        [FieldOffset(0x48)] public long StatNames3;
        [FieldOffset(0x58)] public long StatName4;
        [FieldOffset(0x60)] public int Domain;
        [FieldOffset(0x64)] public long UserFriendlyName;
        [FieldOffset(0x6C)] public int AffixType;
        [FieldOffset(0x70)] public long Group;
        [FieldOffset(0x78)] public int StatRange1;
        [FieldOffset(0x7C)] public int StatRange2;
        [FieldOffset(0x80)] public int StatRange3;
        [FieldOffset(0x84)] public int StatRange4;
        [FieldOffset(0x88)] public int StatRange5;
        [FieldOffset(0x8C)] public int StatRange6;
        [FieldOffset(0x90)] public int StatRange7;
        [FieldOffset(0x94)] public int StatRange8;
        [FieldOffset(0x98)] public long Tags;
        [FieldOffset(0xA0)] public long ta;
        [FieldOffset(0xA8)] public int TagChances;
        [FieldOffset(0xB0)] public long tc;
		[FieldOffset(0xB8)] public long BuffDefinitionsPtr;
		[FieldOffset(0xC0)] public long BuffDefinitions;
		[FieldOffset(0xC8)] public int BuffValue;
		[FieldOffset(0xCC)] public long tgcCount;
		[FieldOffset(0xC4)] public long tgcPtr;
		[FieldOffset(0x1AC)] public byte IsEssence;
        [FieldOffset(0x1C5)] public long Tier;
    }
}
