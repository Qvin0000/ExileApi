namespace ExileCore.PoEMemory.MemoryObjects
{
    public class HideoutWrapper : RemoteMemoryObject
    {
        public string Name => M.ReadStringU(M.Read<long>(Address));
        public WorldArea WorldArea1 => TheGame.Files.WorldAreas.GetByAddress(M.Read<long>(Address + 0x10));
        public WorldArea WorldArea2 => TheGame.Files.WorldAreas.GetByAddress(M.Read<long>(Address + 0x30));
        public WorldArea WorldArea3 => TheGame.Files.WorldAreas.GetByAddress(M.Read<long>(Address + 0x40));
    }
}
