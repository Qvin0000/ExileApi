using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory.Atlas
{
    public class AtlasNodes : UniversalFileWrapper<AtlasNode>
    {
        public AtlasNodes(IMemory mem, Func<long> address) : base(mem, address)
        {
        }

        public IList<AtlasNode> EntriesList
        {
            get
            {
                CheckCache();
                return CachedEntriesList.ToList();
            }
        }

        public AtlasNode GetByAddress(long address)
        {
            return base.GetByAddress(address);
        }
    }
}
