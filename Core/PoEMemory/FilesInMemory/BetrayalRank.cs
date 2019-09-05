using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoEMemory.FilesInMemory
{
    public class BetrayalRank : RemoteMemoryObject
    {
        public string Id => M.ReadStringU(M.Read<long>(Address));
        public string Name => M.ReadStringU(M.Read<long>(Address + 0x8));
        public int Unknown => M.Read<int>(Address + 0x10);
        public string Art => M.ReadStringU(M.Read<long>(Address + 0x14));

        public override string ToString() => Name;
    }
}