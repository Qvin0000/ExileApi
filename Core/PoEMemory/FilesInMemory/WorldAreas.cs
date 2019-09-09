using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Interfaces;

namespace PoEMemory.FilesInMemory
{
    public class WorldAreas : UniversalFileWrapper<WorldArea>
    {
        public Dictionary<int, WorldArea> AreasIndexDictionary { get; } = new Dictionary<int, WorldArea>();

        public WorldAreas(IMemory m, Func<long> address) : base(m, address) { }

        public WorldArea GetAreaByAreaId(int index) {
            CheckCache();

            AreasIndexDictionary.TryGetValue(index, out var area);
            return area;
        }

        public WorldArea GetAreaByAreaId(string id) {
            CheckCache();
            return AreasIndexDictionary.First(area => area.Value.Id == id).Value;
        }

        public int IndexCounter;

        protected override void EntryAdded(long addr, WorldArea entry)
        {
            entry.Index = IndexCounter++;
            AreasIndexDictionary.Add(entry.Index, entry);
        }
    }
}