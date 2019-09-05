using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoEMemory.FilesInMemory
{
    public class BetrayalTarget : RemoteMemoryObject
    {
        public string Id => M.ReadStringU(M.Read<long>(Address));
        public MonsterVariety MonsterVariety => TheGame.Files.MonsterVarieties.GetByAddress(M.Read<long>(Address + 0x20));
        public string Art => M.ReadStringU(M.Read<long>(Address + 0x38));
        public string FullName => M.ReadStringU(M.Read<long>(Address + 0x51));
        public string Name => M.ReadStringU(M.Read<long>(Address + 0x61));

        public override string ToString() => Name;
    }
}