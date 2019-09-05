using SharpDX;

namespace PoEMemory
{
    public class AtlasNode : RemoteMemoryObject
    {
        private WorldArea area;
        public WorldArea Area => area != null ? area : area = TheGame.Files.WorldAreas.GetByAddress(M.Read<long>(Address + 0x08));

        private float posX = -1;
        public float PosX => posX != -1 ? posX : posX = M.Read<float>(Address + 0x10);

        private float posY = -1;
        public float PosY => posY != -1 ? posY : posY = M.Read<float>(Address + 0x14);

        public Vector2 Pos => new Vector2(PosX, PosY);

        private string text;
        public string FlavourText => text != null ? text : text = M.ReadStringU(M.Read<long>(Address + 0x44));

        public bool IsUniqueMap
        {
            get
            {
                var uniqTest = M.ReadStringU(M.Read<long>(Address + 0x3c, 0));
                return !string.IsNullOrEmpty(uniqTest) && uniqTest.Contains("Uniq");
            }
        }

        public override string ToString() => $"{Area.Name}, PosX: {PosX}, PosY: {PosY}, Text: {FlavourText}";
    }
}