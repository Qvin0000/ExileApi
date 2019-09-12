using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory
{
    public class Quests : UniversalFileWrapper<Quest>
    {
        public Quests(IMemory game, Func<long> address) : base(game, address)
        {
        }

        public IList<Quest> EntriesList
        {
            get
            {
                CheckCache();
                return CachedEntriesList.ToList();
            }
        }

        public Quest GetByAddress(long address)
        {
            return base.GetByAddress(address);
        }
    }
}
