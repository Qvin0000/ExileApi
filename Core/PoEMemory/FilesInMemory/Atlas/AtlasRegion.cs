namespace ExileCore.PoEMemory.FilesInMemory.Atlas
{
    public class AtlasRegion : RemoteMemoryObject
    {
        public int Index { get; internal set; }
        public string Id => M.ReadStringU(M.Read<long>(Address));
        public string Name => M.ReadStringU(M.Read<long>(Address + 0x8));
        public int Unknown => M.Read<int>(Address + 0x10);

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
