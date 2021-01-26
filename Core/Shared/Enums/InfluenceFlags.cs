using System;

namespace ExileCore.Shared.Enums
{
	[Flags]
	public enum Influence : byte
	{
		Shaper = 1,
		Elder = 2,
		Crusader = 4,
		Redeemer = 8,
		Hunter = 16,
		Warlord = 32,
	}
}
