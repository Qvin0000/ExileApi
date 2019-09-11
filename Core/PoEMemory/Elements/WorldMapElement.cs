namespace ExileCore.PoEMemory.Elements
{
    public class WorldMapElement : Element
    {
        public Element Panel => GetObject<Element>(M.Read<long>(Address + 0xAB8, 0xC10));
    }
}
