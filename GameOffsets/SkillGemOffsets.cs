using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct SkillGemOffsets
    {
        // Token: 0x04004664 RID: 18020
        [FieldOffset(0x0)] public InitObjectOffsets Head;
        [FieldOffset(0x28)] public long AdvanceInformation;
        [FieldOffset(0x30)] public uint TotalExpGained;
        [FieldOffset(0x34)] public uint Level;
        [FieldOffset(0x38)] public uint ExperiencePrevLevel;
        [FieldOffset(0x3C)] public uint ExperienceMaxLevel;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct GemInformation
    {
        [FieldOffset(0x30)] public int SocketColor;
        [FieldOffset(0x48)] public int MaxLevel;
        [FieldOffset(0x4c)] public int LimitLevel;
    }
}
