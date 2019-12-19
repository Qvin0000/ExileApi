namespace ExileCore.PoEMemory.FilesInMemory.Metamorph
{
    public class MetamorphRewardType : RemoteMemoryObject
    {
        public string Id => M.ReadStringU(M.Read<long>(Address + 0x0), 255);
        public string Art => M.ReadStringU(M.Read<long>(Address + 0x8), 255);
        public string Name => M.ReadStringU(M.Read<long>(Address + 0x10), 255);
        //0x18 UINT unknown
        //0x20 ptr -> ptr[0x8] achievement
        public override string ToString()
        {
            return $"{Id}: {Name}";
        }
    }
}
