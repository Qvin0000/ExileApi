using ExileCore.PoEMemory.FilesInMemory.Atlas;
using SharpDX;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class AtlasNode : RemoteMemoryObject
    {
        private WorldArea area;
        private float posX = -1;
        private float posY = -1;
        private string text;
        public WorldArea Area => area != null ? area : area = TheGame.Files.WorldAreas.GetByAddress(M.Read<long>(Address + 0x8));
        public float PosX => posX != -1 ? posX : posX = M.Read<float>(Address + 0x11D);
        public float PosY => posY != -1 ? posY : posY = M.Read<float>(Address + 0x121);
        public Vector2 DefaulPos => new Vector2(PosX, PosY);
        public Vector2 Pos => GetPosByLayer(1);

        //Atlas region: 0x4D

        public string FlavourText => text != null ? text : text = M.ReadStringU(M.Read<long>(Address + 0x44));

        public AtlasRegion AtlasRegion => TheGame.Files.AtlasRegions.GetByAddress(M.Read<long>(Address + 0x4D));

        public Vector2 GetPosByLayer(int layer)
        {
            const int X_START = 0xB9;
            const int Y_START = 0xCD;

            var x = M.Read<float>(Address + X_START + layer * sizeof(float));
            var y = M.Read<float>(Address + Y_START + layer * sizeof(float));
            return new Vector2(x, y);
        }

        public int GetTierByLayer(int layer)
        {
            const int TIER_START = 0xA5;

            return M.Read<int>(Address + TIER_START + layer * sizeof(int));
        }

        public bool IsUniqueMap
        {
            get
            {
                var uniqTest = M.ReadStringU(M.Read<long>(Address + 0x3c, 0));
                return !string.IsNullOrEmpty(uniqTest) && uniqTest.Contains("Uniq");
            }
        }

        public override string ToString()
        {
            return $"{Area.Name}, PosX: {PosX}, PosY: {PosY}, Text: {FlavourText}";
        }
    }
}
