using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Interfaces;

namespace PoEMemory.FilesInMemory
{
    public class AtlasNodes : UniversalFileWrapper<AtlasNode>
    {
        public AtlasNodes(IMemory mem, Func<long> address) : base(mem, address) { }

        public IList<AtlasNode> EntriesList
        {
            get
            {
                CheckCache();
                return CachedEntriesList.ToList();
            }
        }

        public AtlasNode GetByAddress(long address) => base.GetByAddress(address);
    }
}