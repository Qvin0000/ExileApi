using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SkillBarIdsStruct
    {
        public ushort SkillBar1;
        public ushort SkillBar2;
        public ushort SkillBar3;
        public ushort SkillBar4;
        public ushort SkillBar5;
        public ushort SkillBar6;
        public ushort SkillBar7;
        public ushort SkillBar8;
        public ushort SkillBar9;
        public ushort SkillBar10;
        public ushort SkillBar11;
        public ushort SkillBar12;
        public ushort SkillBar13;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ServerDataOffsets
    {
        public const int Skip = 0x5000;
        public const int ATLAS_REGION_UPGRADES = 0x7782;
		[FieldOffset(0)] public long MasterAreas;
		[FieldOffset(0x7068 - Skip)] public NativePtrArray PassiveSkillIds;
        [FieldOffset(0x7108 - Skip)] public byte PlayerClass;
        [FieldOffset(0x710C - Skip)] public int CharacterLevel;
        [FieldOffset(0x7110 - Skip)] public int PassiveRefundPointsLeft;
        [FieldOffset(0x7114 - Skip)] public int QuestPassiveSkillPoints;
        [FieldOffset(0x7118 - Skip)] public int FreePassiveSkillPointsLeft;//TODO: 3.8.1 fix me
        [FieldOffset(0x711C - Skip)] public int TotalAscendencyPoints;
        [FieldOffset(0x7120 - Skip)] public int SpentAscendencyPoints;
        [FieldOffset(0x7188 - Skip)] public byte NetworkState;
		[FieldOffset(0x71A0 - Skip)] public NativeStringU League;
        [FieldOffset(0x7220 - Skip)] public float TimeInGame;
        [FieldOffset(0x7228 - Skip)] public int Latency;
		[FieldOffset(0x7238 - Skip)] public NativePtrArray PlayerStashTabs;
		[FieldOffset(0x7250 - Skip)] public NativePtrArray GuildStashTabs;
		[FieldOffset(0x7350 - Skip)] public byte PartyStatusType;
		[FieldOffset(0x7360 - Skip)] public byte PartyAllocationType;
		[FieldOffset(0x7360 - Skip)] public long GuildName;
		[FieldOffset(0x73F0 - Skip)] public SkillBarIdsStruct SkillBarIds;
        [FieldOffset(0x7440 - Skip)] public NativePtrArray NearestPlayers;
        [FieldOffset(0x7548 - Skip)] public NativePtrArray PlayerInventories;
        [FieldOffset(0x7618 - Skip)] public NativePtrArray NPCInventories;
        [FieldOffset(0x76D0 - Skip)] public NativePtrArray GuildInventories;
        [FieldOffset(0x7828 - Skip)] public ushort TradeChatChannel;
        [FieldOffset(0x7830 - Skip)] public ushort GlobalChatChannel;
		[FieldOffset(0x787C - Skip)] public ushort LastActionId;//Do we need this?
		[FieldOffset(0x78F0 - Skip)] public long CompletedMaps;//search for a LONG value equals to your current amount of completed maps. Pointer will be under this offset
        [FieldOffset(0x78F8 - Skip)] public long BonusCompletedAreas;
        [FieldOffset(0x7440 - Skip)] public long ElderInfluencedAreas;
        [FieldOffset(0x7660 - Skip)] public long ElderGuardiansAreas; //Maybe wrong not tested
        [FieldOffset(0x7930 - Skip)] public long ShapedAreas; //Maybe wrong not tested
		[FieldOffset(0x842C - Skip)] public byte MonsterLevel;
        [FieldOffset(0x842D - Skip)] public byte MonstersRemaining;
        [FieldOffset(0x84E0 - Skip)] public ushort CurrentSulphiteAmount; //Maybe wrong not tested
        [FieldOffset(0x84EC - Skip)] public int CurrentAzuriteAmount;
    }
}
