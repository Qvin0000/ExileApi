using System;
using System.Collections.Generic;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory.Atlas
{
    public class AtlasRegions : UniversalFileWrapper<AtlasRegion>
    {
        public int IndexCounter;

        public AtlasRegions(IMemory mem, Func<long> address) : base(mem, address)
        {
        }

        public Dictionary<int, AtlasRegion> RegionIndexDictionary { get; } = new Dictionary<int, AtlasRegion>();

        protected override void EntryAdded(long addr, AtlasRegion entry)
        {
            entry.Index = IndexCounter++;
            RegionIndexDictionary.Add(entry.Index, entry);
        }
    }
}
