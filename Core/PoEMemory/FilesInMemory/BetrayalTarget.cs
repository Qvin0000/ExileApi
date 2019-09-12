using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.FilesInMemory
{
    public class BetrayalTarget : RemoteMemoryObject
    {
        public string Id => M.ReadStringU(M.Read<long>(Address));
        public MonsterVariety MonsterVariety => TheGame.Files.MonsterVarieties.GetByAddress(M.Read<long>(Address + 0x20));
        public string Art => M.ReadStringU(M.Read<long>(Address + 0x38));
        public string FullName => M.ReadStringU(M.Read<long>(Address + 0x51));
        public string Name => M.ReadStringU(M.Read<long>(Address + 0x61));

        public override string ToString()
        {
            return Name;
        }
    }
}
