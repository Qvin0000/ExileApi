namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct SkillGem
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0020)] public long SkillGemInternalPtr; //SkillGemInternalStruct
        [FieldOffset(0x0028)] public uint TotalExpGained;
        [FieldOffset(0x002C)] public uint Level;
        [FieldOffset(0x0030)] public uint ExperiencePrevLevel;
        [FieldOffset(0x0034)] public uint ExperienceMaxLevel;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct SkillGemInternalStruct
    {
        [FieldOffset(0x0030)] public int SocketColor;
        [FieldOffset(0x0040)] public long SkillGemDatPtr; //SkillGem.Dat
        [FieldOffset(0x0048)] public int MaxLevel;
        [FieldOffset(0x004C)] public int LimitLevel;
    }
}