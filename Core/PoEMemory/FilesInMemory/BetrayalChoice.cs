namespace ExileCore.PoEMemory.FilesInMemory
{
    public class BetrayalChoice : RemoteMemoryObject
    {
        public string Id => M.ReadStringU(M.Read<long>(Address));
        public string Name => M.ReadStringU(M.Read<long>(Address + 0x8));

        public override string ToString()
        {
            return Name;
        }
    }
}
