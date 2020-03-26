using ExileCore.Shared.Helpers;
using GameOffsets.Native;
using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.Components
{
    public class Base : Component
    {
        //x20 - some strings about item
        private string _name;
        public string Name => _name ?? (_name = M.Read<NativeStringU>(Address + 0x10, 0x18).ToString(M));
        public int ItemCellsSizeX => M.Read<int>(Address + 0x10, 0x10);
        public int ItemCellsSizeY => M.Read<int>(Address + 0x10, 0x14);
		private Influence InfluenceFlag => (Influence)M.Read<byte>(Address + 0xD8);
		public bool isShaper => (InfluenceFlag & Influence.Shaper) == Influence.Shaper;
		public bool isElder => (InfluenceFlag & Influence.Elder) == Influence.Elder;
		public bool isCrusader => (InfluenceFlag & Influence.Crusader) == Influence.Crusader;
		public bool isHunter => (InfluenceFlag & Influence.Hunter) == Influence.Hunter;
		public bool isRedeemer => (InfluenceFlag & Influence.Redeemer) == Influence.Redeemer;
		public bool isWarlord => (InfluenceFlag & Influence.Warlord) == Influence.Warlord;
		public bool isCorrupted => M.Read<byte>(Address + 0xDA) == 1;
		public bool isSynthesized => M.Read<byte>(Address + 0xDE) == 1;

		// public bool isFractured => M.Read<byte>(Address + 0x98) == 0;

		// 0x8 - link to base item
		// +0x10 - Name
		// +0x30 - Use hint
		// +0x50 - Link to Data/BaseItemTypes.dat

		// 0xC (+4) fileref to visual identity
	}
}
