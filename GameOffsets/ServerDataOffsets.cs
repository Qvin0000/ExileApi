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
        [FieldOffset(0x6E98 - Skip)] public NativeStringU League;
        [FieldOffset(0x6B70 - Skip)] public NativePtrArray PassiveSkillIds;
        [FieldOffset(0x6BC0 - Skip)] public byte PlayerClass;
        [FieldOffset(0x6BC4 - Skip)] public int CharacterLevel;
        [FieldOffset(0x6BC8 - Skip)] public int PassiveRefundPointsLeft;
        [FieldOffset(0x6BCC - Skip)] public int QuestPassiveSkillPoints;
        [FieldOffset(0x63D0 - Skip)] public int FreePassiveSkillPointsLeft;//TODO: 3.8.1 fix me
        [FieldOffset(0x6BD4 - Skip)] public int TotalAscendencyPoints;
        [FieldOffset(0x6BD8 - Skip)] public int SpentAscendencyPoints;
        [FieldOffset(0x6DD8 - Skip)] public byte PartyStatusType;
        [FieldOffset(0x6F00 - Skip)] public byte NetworkState;
        [FieldOffset(0x6DF8 - Skip)] public byte PartyAllocationType;
        [FieldOffset(0x6C98 - Skip)] public float TimeInGame;
        [FieldOffset(0x6CA0 - Skip)] public int Latency;
        [FieldOffset(0x7168 - Skip)] public SkillBarIdsStruct SkillBarIds;
        [FieldOffset(0x6CB0 - Skip)] public NativePtrArray PlayerStashTabs;
        [FieldOffset(0x73E0 - Skip)] public NativePtrArray GuildStashTabs;
        [FieldOffset(0x71C0 - Skip)] public NativePtrArray NearestPlayers;
        [FieldOffset(0x72C0 - Skip)] public NativePtrArray PlayerInventories;
        [FieldOffset(0x7390 - Skip)] public NativePtrArray NPCInventories;
        [FieldOffset(0x7460 - Skip)] public NativePtrArray GuildInventories;
        [FieldOffset(0x7270 - Skip)] public ushort TradeChatChannel;
        [FieldOffset(0x7278 - Skip)] public ushort GlobalChatChannel;
        [FieldOffset(0x76A0 - Skip)] public long CompletedMaps;//search for a LONG value equals to your current amount of completed maps. Pointer will be under this offset
        [FieldOffset(0x7660 - Skip)] public long BonusCompletedAreas;
        [FieldOffset(0x7440 - Skip)] public long ElderInfluencedAreas;
        [FieldOffset(0)] public long MasterAreas;
        [FieldOffset(0x7660 - Skip)] public long ElderGuardiansAreas; //Maybe wrong not tested
        [FieldOffset(0x7660 - Skip)] public long ShapedAreas; //Maybe wrong not tested
        [FieldOffset(0x6E77 - Skip)] public ushort LastActionId;//Do we need this?
        [FieldOffset(0x7E5C - Skip)] public byte MonsterLevel;
        [FieldOffset(0x7E5D - Skip)] public byte MonstersRemaining;
        [FieldOffset(0x7F10 - Skip)] public ushort CurrentSulphiteAmount; //Maybe wrong not tested
        [FieldOffset(0x7F00 - Skip)] public int CurrentAzuriteAmount;
    }
}
