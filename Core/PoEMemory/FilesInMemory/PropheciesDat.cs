using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Interfaces;

namespace PoEMemory.FilesInMemory
{
    public class PropheciesDat : UniversalFileWrapper<ProphecyDat>
    {
        private Dictionary<int, ProphecyDat> ProphecyIndexDictionary = new Dictionary<int, ProphecyDat>();

        public PropheciesDat(IMemory m, Func<long> address) : base(m, address) { }

        private bool loaded;

        public ProphecyDat GetProphecyById(int index) {
            CheckCache();

            if (!loaded)
            {
                foreach (var prophecyDat in EntriesList) EntryAdded(prophecyDat.Address, prophecyDat);

                loaded = true;
            }

            ProphecyIndexDictionary.TryGetValue(index, out var prophecy);
            return prophecy;
        }

        private int IndexCounter;

        protected void EntryAdded(long addr, ProphecyDat entry) {
            entry.Index = IndexCounter++;
            ProphecyIndexDictionary.Add(entry.ProphecyId, entry);
        }

        public IList<ProphecyDat> EntriesList => base.EntriesList.ToList();
        public ProphecyDat GetByAddress(long address) => base.GetByAddress(address);
    }
}