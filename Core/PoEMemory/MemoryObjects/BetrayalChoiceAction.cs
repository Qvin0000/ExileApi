using PoEMemory;
using PoEMemory.FilesInMemory;

namespace Exile.PoEMemory.MemoryObjects
{
    public class BetrayalChoiceAction : RemoteMemoryObject
    {
        public string Id => M.ReadStringU(M.Read<long>(Address));
        public BetrayalChoice Choice => TheGame.Files.BetrayalChoises.GetByAddress(M.Read<long>(Address + 0x10));

        public override string ToString() => $"{Id} ({Choice.Name})";
    }
}