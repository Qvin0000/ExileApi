using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory
{
    public class PropheciesDat : UniversalFileWrapper<ProphecyDat>
    {
        private int IndexCounter;
        private bool loaded;
        private readonly Dictionary<int, ProphecyDat> ProphecyIndexDictionary = new Dictionary<int, ProphecyDat>();

        public PropheciesDat(IMemory m, Func<long> address) : base(m, address)
        {
        }

        public IList<ProphecyDat> EntriesList => base.EntriesList.ToList();

        public ProphecyDat GetProphecyById(int index)
        {
            CheckCache();

            if (!loaded)
            {
                foreach (var prophecyDat in EntriesList)
                {
                    EntryAdded(prophecyDat.Address, prophecyDat);
                }

                loaded = true;
            }

            ProphecyIndexDictionary.TryGetValue(index, out var prophecy);
            return prophecy;
        }

        protected void EntryAdded(long addr, ProphecyDat entry)
        {
            entry.Index = IndexCounter++;
            ProphecyIndexDictionary.Add(entry.ProphecyId, entry);
        }

        public ProphecyDat GetByAddress(long address)
        {
            return base.GetByAddress(address);
        }
    }
}
