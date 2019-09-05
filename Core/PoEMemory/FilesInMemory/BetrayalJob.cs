using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoEMemory.FilesInMemory
{
    public class BetrayalJob : RemoteMemoryObject
    {
        public string Id => M.ReadStringU(M.Read<long>(Address));
        public string Name => M.ReadStringU(M.Read<long>(Address + 0x8));
        public string Art => M.ReadStringU(M.Read<long>(Address + 0x20));

        public override string ToString() => Name;
    }
}