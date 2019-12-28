using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory
{
    public class WorldAreas : UniversalFileWrapper<WorldArea>
    {
        private int _indexCounter;

        public WorldAreas(IMemory m, Func<long> address) : base(m, address)
        {
        }

        public Dictionary<int, WorldArea> AreasIndexDictionary { get; } = new Dictionary<int, WorldArea>();

        public WorldArea GetAreaByAreaId(int index)
        {
            CheckCache();

            AreasIndexDictionary.TryGetValue(index, out var area);
            return area;
        }

        public WorldArea GetAreaByAreaId(string id)
        {
            CheckCache();
            return AreasIndexDictionary.First(area => area.Value.Id == id).Value;
        }

        protected override void EntryAdded(long addr, WorldArea entry)
        {
            entry.Index = _indexCounter++;
            AreasIndexDictionary.Add(entry.Index, entry);
        }
    }
}
