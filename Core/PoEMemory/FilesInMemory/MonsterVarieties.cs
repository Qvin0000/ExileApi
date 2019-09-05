using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Interfaces;

namespace PoEMemory.FilesInMemory
{
    public class MonsterVarieties : UniversalFileWrapper<MonsterVariety>
    {
        private readonly Dictionary<string, MonsterVariety> MonsterVarietyMetadataDictionary = new Dictionary<string, MonsterVariety>();

        public MonsterVarieties(IMemory m, Func<long> address) : base(m, address) { }

        public MonsterVariety TranslateFromMetadata(string path) {
            CheckCache();
            MonsterVarietyMetadataDictionary.TryGetValue(path, out var result);
            return result;
        }

        protected void EntryAdded(long addr, MonsterVariety entry) => MonsterVarietyMetadataDictionary.Add(entry.VarietyId, entry);

        public IList<MonsterVariety> EntriesList => base.EntriesList.ToList();
        public MonsterVariety GetByAddress(long address) => base.GetByAddress(address);
    }
}