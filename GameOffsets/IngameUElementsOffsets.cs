using System.Runtime.InteropServices;

namespace GameOffsets
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct IngameUElementsOffsets
	{
		[FieldOffset(0x210)] public long GetQuests;
		[FieldOffset(0x250)] public long GameUI;
        [FieldOffset(0x380)] public long Mouse;
        [FieldOffset(0x388)] public long SkillBar;
        [FieldOffset(0x390)] public long HiddenSkillBar;
        [FieldOffset(0x490)] public long QuestTracker;
        [FieldOffset(0x4F8)] public long OpenLeftPanel;
        [FieldOffset(0x500)] public long OpenRightPanel;
        [FieldOffset(0x530)] public long InventoryPanel;
        [FieldOffset(0x538)] public long StashElement;
        [FieldOffset(0x560)] public long TreePanel;
        [FieldOffset(0x568)] public long AtlasPanel;
        [FieldOffset(0x598)] public long WorldMap;
        [FieldOffset(0x5C0)] public long Map;
        [FieldOffset(0x5C8)] public long itemsOnGroundLabelRoot;
        [FieldOffset(0x660)] public long PurchaseWindow;
        [FieldOffset(0x668)] public long SellWindow;
        [FieldOffset(0x6A8)] public long MapDeviceWindow;
        [FieldOffset(0x700)] public long IncursionWindow;
        [FieldOffset(0x720)] public long DelveWindow;
        [FieldOffset(0x740)] public long BetrayalWindow;
        [FieldOffset(0x748)] public long ZanaMissionChoice;
        [FieldOffset(0x758)] public long CraftBenchWindow;
        [FieldOffset(0x760)] public long UnveilWindow;
        [FieldOffset(0x790)] public long SynthesisWindow;
        [FieldOffset(0x7A0)] public long MetamorphWindow;
        [FieldOffset(0x800)] public long AreaInstanceUi;
        [FieldOffset(0x8D0)] public long InvitesPanel;
        [FieldOffset(0x948)] public long GemLvlUpPanel;
        [FieldOffset(0x9F8)] public long ItemOnGroundTooltip;
        [FieldOffset(0x0/*0xF18*/)] public long MapTabWindowStartPtr;//TOFO: Fixme. Cause reading errors
	}
}
