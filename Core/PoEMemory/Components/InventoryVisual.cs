namespace ExileCore.PoEMemory.Components
{
    public class InventoryVisual : RemoteMemoryObject
    {
        public string Name => M.ReadStringU(M.Read<long>(Address));
        public string Texture => M.ReadStringU(M.Read<long>(Address + 0x8));
        public string Model => M.ReadStringU(M.Read<long>(Address + 0x10));
    }
}
