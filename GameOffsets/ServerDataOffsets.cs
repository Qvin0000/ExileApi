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
    public const int Skip = 20480;
    [FieldOffset(0x63E8-Skip)]public NativeStringU League2;
    [FieldOffset(0x6340-Skip)]public NativePtrArray PassiveSkillIds;
    [FieldOffset(0x63B0-Skip)]public byte PlayerClass;
    [FieldOffset(0x6394-Skip)]public int CharacterLevel;
    [FieldOffset(0x6398-Skip)]public int PassiveRefundPointsLeft;
    [FieldOffset(0x639C-Skip)]public int QuestPassiveSkillPoints;
    [FieldOffset(0x63D0-Skip)]public int FreePassiveSkillPointsLeft;
    [FieldOffset(0x63A4-Skip)]public int TotalAscendencyPoints;
    [FieldOffset(0x63A8-Skip)]public int SpentAscendencyPoints;
    [FieldOffset(0x63D0-Skip)]public byte NetworkState;
    [FieldOffset(0x65C8-Skip)]public byte PartyAllocationType;
    [FieldOffset(0x6468-Skip)]public float TimeInGame;
    [FieldOffset(0x6470-Skip)]public int Latency;
    [FieldOffset(0x6480-Skip)]public NativePtrArray PlayerStashTabs;
    [FieldOffset(0x6498-Skip)]public NativePtrArray GuildStashTabs;
    [FieldOffset(0x6690-Skip)]public NativePtrArray NearestPlayers;
    [FieldOffset(0x6F30-Skip)]public NativePtrArray PlayerInventories;
    [FieldOffset(0x6828-Skip)]public NativePtrArray NPCInventories;
    [FieldOffset(0x68E0-Skip)]public NativePtrArray GuildInventories;
    [FieldOffset(0x69F8-Skip)]public ushort TradeChatChannel;
    [FieldOffset(0x6A00-Skip)]public ushort GlobalChatChannel;
    [FieldOffset(0x6A4C-Skip)]public ushort LastActionId;
    [FieldOffset(0x75AC-Skip)]public byte MonsterLevel;
    [FieldOffset(0x75AD-Skip)]public byte MonstersRemaining;
    [FieldOffset(0x7644-Skip)]public ushort CurrentSulphiteAmount;
    [FieldOffset(0x7650-Skip)]public int CurrentAzuriteAmount;
    [FieldOffset(0x6638-Skip)] public SkillBarIdsStruct SkillBarIds;
  }
}
