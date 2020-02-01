using System.Runtime.InteropServices;

namespace GameOffsets
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct IngameUElementsOffsets
	{
		[FieldOffset(0x210)] public long GetQuests;
		[FieldOffset(0x250)] public long GameUI;
		[FieldOffset(0x368)] public long Mouse;
		[FieldOffset(0x378)] public long HiddenSkillBar;
		[FieldOffset(0x370)] public long SkillBar;
		[FieldOffset(0x478)] public long QuestTracker;
		[FieldOffset(0x4E0 /*4F0*/)] public long OpenLeftPanel;
		[FieldOffset(0x4E8 /*4F8*/)] public long OpenRightPanel;
		[FieldOffset(0x518)] public long InventoryPanel;
		[FieldOffset(0x520)] public long StashElement;
		[FieldOffset(0x548)] public long TreePanel;
		[FieldOffset(0x550)] public long AtlasPanel;
		[FieldOffset(0x5A0)] public long Map;
		[FieldOffset(0x5A8)] public long itemsOnGroundLabelRoot;
		[FieldOffset(0x640)] public long PurchaseWindow;
		[FieldOffset(0x648)] public long SellWindow;
		[FieldOffset(0x650)] public long TradeWindow;
		[FieldOffset(0x688)] public long MapDeviceWindow;
		[FieldOffset(0x6E0)] public long IncursionWindow;
		[FieldOffset(0x700)] public long DelveWindow;
		[FieldOffset(0x720)] public long BetrayalWindow;
		[FieldOffset(0x728)] public long ZanaMissionChoice;
		[FieldOffset(0x738)] public long CraftBenchWindow;
		[FieldOffset(0x740)] public long UnveilWindow;
		[FieldOffset(0x768)] public long SynthesisWindow;
		[FieldOffset(0x778)] public long MetamorphWindow;
		[FieldOffset(0x7C8)] public long AreaInstanceUi;
		[FieldOffset(0x8E8)] public long GemLvlUpPanel;
		[FieldOffset(0x8A0)] public long InvitesPanel;
		[FieldOffset(0x990)] public long ItemOnGroundTooltip;
		[FieldOffset(0x0/*0xCC0*/)] public long WorldMap;//TOFO: Fixme. Cause reading errors
		[FieldOffset(0x0/*0xF18*/)] public long MapTabWindowStartPtr;//TOFO: Fixme. Cause reading errors
	}
}
