namespace ExileCore.PoEMemory.FilesInMemory
{
    public class BetrayalJob : RemoteMemoryObject
    {
        public string Id => M.ReadStringU(M.Read<long>(Address));
        public string Name => M.ReadStringU(M.Read<long>(Address + 0x8));
        public string Art => M.ReadStringU(M.Read<long>(Address + 0x20));

        public override string ToString()
        {
            return Name;
        }
    }
}
