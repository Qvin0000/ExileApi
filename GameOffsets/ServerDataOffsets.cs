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
        [FieldOffset(0x6BA8 - Skip)] public NativeStringU League;
        [FieldOffset(0x6B20 - Skip)] public NativePtrArray PassiveSkillIds;
        [FieldOffset(0x6B60 - Skip)] public byte PlayerClass;
        [FieldOffset(0x6BC4 - Skip)] public int CharacterLevel;
        [FieldOffset(0x6B48 - Skip)] public int PassiveRefundPointsLeft;//TODO: 3.8.1 fix me
        [FieldOffset(0x6B4C - Skip)] public int QuestPassiveSkillPoints;//TODO: 3.8.1 fix me
        [FieldOffset(0x63D0 - Skip)] public int FreePassiveSkillPointsLeft;//TODO: 3.8.1 fix me
        [FieldOffset(0x6B54 - Skip)] public int TotalAscendencyPoints;//TODO: 3.8.1 fix me
        [FieldOffset(0x6B58 - Skip)] public int SpentAscendencyPoints;//TODO: 3.8.1 fix me
        [FieldOffset(0x6D58 - Skip)] public byte PartyStatusType;//TODO: 3.8.1 fix me
        [FieldOffset(0x6B80 - Skip)] public byte NetworkState;//TODO: 3.8.1 fix me
        [FieldOffset(0x6D78 - Skip)] public byte PartyAllocationType;//TODO: 3.8.1 fix me
        [FieldOffset(0x6C98 - Skip)] public float TimeInGame;
        [FieldOffset(0x6CA0 - Skip)] public int Latency;
        [FieldOffset(0x6CB0)] public NativePtrArray PlayerStashTabs;
        [FieldOffset(0x6C58)] public NativePtrArray GuildStashTabs;
        [FieldOffset(0x6690 - Skip)] public NativePtrArray NearestPlayers;//TODO: 3.8.1 fix me
        [FieldOffset(0x6FB0 - Skip)] public NativePtrArray PlayerInventories;
        [FieldOffset(0x7070 - Skip)] public NativePtrArray NPCInventories;
        [FieldOffset(0x7130 - Skip)] public NativePtrArray GuildInventories;
        [FieldOffset(0x7270 - Skip)] public ushort TradeChatChannel;
        [FieldOffset(0x7278 - Skip)] public ushort GlobalChatChannel;
        [FieldOffset(0x72C0 - Skip)] public long CompletedMaps;//TODO: 3.8.1 fix me
        [FieldOffset(0x7340 - Skip)] public long BonusCompletedAreas;//TODO: 3.8.1 fix me
        [FieldOffset(0x73C0 - Skip)] public long ElderInfluencedAreas;//TODO: 3.8.1 fix me
        [FieldOffset(0)] public long MasterAreas;
        [FieldOffset(0)] public long ElderGuardiansAreas;
        [FieldOffset(0)] public long ShapedAreas;
        [FieldOffset(0x6E77 - Skip)] public ushort LastActionId;//Do we need this?
        [FieldOffset(0x7DDC - Skip)] public byte MonsterLevel;//TODO: 3.8.1 fix me
        [FieldOffset(0x7DDD - Skip)] public byte MonstersRemaining;//TODO: 3.8.1 fix me
        [FieldOffset(0x7F00 - Skip)] public ushort CurrentSulphiteAmount;
        [FieldOffset(0x7650 - Skip)] public int CurrentAzuriteAmount;//TODO: 3.8.1 fix me
        [FieldOffset(0x6E68 - Skip)] public SkillBarIdsStruct SkillBarIds;
    }
}
