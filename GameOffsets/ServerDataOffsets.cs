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
		//[FieldOffset(0x7068 - Skip)] public NativePtrArray PassiveSkillIds;
        [FieldOffset(0x72B0 - Skip)] public byte PlayerClass;
        //[FieldOffset(0x710C - Skip)] public int CharacterLevel;
        //[FieldOffset(0x7110 - Skip)] public int PassiveRefundPointsLeft;
        //[FieldOffset(0x7114 - Skip)] public int QuestPassiveSkillPoints;
        //[FieldOffset(0x7118 - Skip)] public int FreePassiveSkillPointsLeft;//TODO: 3.8.1 fix me
        //[FieldOffset(0x711C - Skip)] public int TotalAscendencyPoints;
        //[FieldOffset(0x7120 - Skip)] public int SpentAscendencyPoints;
        [FieldOffset(0x7328 - Skip)] public byte NetworkState;
		[FieldOffset(0x7350 - Skip)] public NativeStringU League;
        [FieldOffset(0x73C0 - Skip)] public float TimeInGame;
        [FieldOffset(0x73C8 - Skip)] public int Latency;
		[FieldOffset(0x73D8 - Skip)] public NativePtrArray PlayerStashTabs;
		[FieldOffset(0x73F0 - Skip)] public NativePtrArray GuildStashTabs;
		[FieldOffset(0x74F0 - Skip)] public byte PartyStatusType;
		[FieldOffset(0x7500 - Skip)] public byte PartyAllocationType;
		[FieldOffset(0x7588 - Skip)] public long GuildName;
		[FieldOffset(0x7590 - Skip)] public SkillBarIdsStruct SkillBarIds;
        [FieldOffset(0x75E8 - Skip)] public NativePtrArray NearestPlayers;
        [FieldOffset(0x76F0 - Skip)] public NativePtrArray PlayerInventories;
        [FieldOffset(0x77C8 - Skip)] public NativePtrArray NPCInventories;
        [FieldOffset(0x7880 - Skip)] public NativePtrArray GuildInventories;
        [FieldOffset(0x79E0 - Skip)] public ushort TradeChatChannel;
        [FieldOffset(0x79E8 - Skip)] public ushort GlobalChatChannel;
		[FieldOffset(0x7A38 - Skip)] public ushort LastActionId;//Do we need this?
		[FieldOffset(0x7AB0 - Skip)] public long CompletedMaps;//search for a LONG value equals to your current amount of completed maps. Pointer will be under this offset
        [FieldOffset(0x7AF0 - Skip)] public long BonusCompletedAreas;
        [FieldOffset(0x7B30 - Skip)] public long AwakenedAreas;
        [FieldOffset(0x85E4 - Skip)] public byte MonsterLevel;
        [FieldOffset(0x85e5 - Skip)] public byte MonstersRemaining;
        [FieldOffset(0x8698 - Skip)] public ushort CurrentSulphiteAmount; //Maybe wrong not tested
        [FieldOffset(0x86A4 - Skip)] public int CurrentAzuriteAmount;
    }
}
