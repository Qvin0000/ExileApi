namespace PoEMemory
{
    public class Quest : RemoteMemoryObject
    {
        private string id;
        public string Id => id != null ? id : id = M.ReadStringU(M.Read<long>(Address), 255);

        public int Act => M.Read<int>(Address + 0x8);

        private string name;
        public string Name => name != null ? name : name = M.ReadStringU(M.Read<long>(Address + 0xc));

        public string Icon => M.ReadStringU(M.Read<long>(Address + 0x18));

        public override string ToString() => $"Id: {Id}, Name: {Name}";
    }
}