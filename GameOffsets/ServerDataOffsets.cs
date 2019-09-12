using GameOffsets.Native;
using System.Runtime.InteropServices;

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
  }
  
  [StructLayout(LayoutKind.Explicit, Pack = 1)]
  public struct ServerDataOffsets
  {
    public const int Skip = 0x5000;
    [FieldOffset(0x6BA8-Skip)]public NativeStringU League;
    [FieldOffset(0x6AF0-Skip)]public NativePtrArray PassiveSkillIds;
    [FieldOffset(0x6B60-Skip)]public byte PlayerClass;
    [FieldOffset(0x6B44-Skip)]public int CharacterLevel;
    [FieldOffset(0x6B48-Skip)]public int PassiveRefundPointsLeft;
    [FieldOffset(0x6B4C-Skip)]public int QuestPassiveSkillPoints;
    [FieldOffset(0x63D0-Skip)]public int FreePassiveSkillPointsLeft;
    [FieldOffset(0x6B54-Skip)]public int TotalAscendencyPoints;
    [FieldOffset(0x6B58-Skip)]public int SpentAscendencyPoints;
    [FieldOffset(0x6D58-Skip)]public byte PartyStatusType;
    [FieldOffset(0x6B80-Skip)]public byte NetworkState;
    [FieldOffset(0x6D78-Skip)]public byte PartyAllocationType;
    [FieldOffset(0x6C18-Skip)]public float TimeInGame;
    [FieldOffset(0x6C20-Skip)]public int Latency;
    [FieldOffset(0x6C30)]public NativePtrArray PlayerStashTabs;
    [FieldOffset(0x6C48)]public NativePtrArray GuildStashTabs;
    [FieldOffset(0x6690-Skip)]public NativePtrArray NearestPlayers;
    [FieldOffset(0x6F30-Skip)]public NativePtrArray PlayerInventories;
    [FieldOffset(0x6828-Skip)]public NativePtrArray NPCInventories;
    [FieldOffset(0x68E0-Skip)]public NativePtrArray GuildInventories;
    [FieldOffset(0x71F0-Skip)]public ushort TradeChatChannel;
    [FieldOffset(0x71F8-Skip)]public ushort GlobalChatChannel;
    [FieldOffset(0x6E77-Skip)]public ushort LastActionId;
    [FieldOffset(0x7DDC-Skip)]public byte MonsterLevel;
    [FieldOffset(0x7DDD-Skip)]public byte MonstersRemaining;
    [FieldOffset(0x7644-Skip)]public ushort CurrentSulphiteAmount;
    [FieldOffset(0x7650-Skip)]public int CurrentAzuriteAmount;
    [FieldOffset(0x6638-Skip)] public SkillBarIdsStruct SkillBarIds;
  }
}
