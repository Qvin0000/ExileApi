using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Interfaces;

namespace PoEMemory.FilesInMemory
{
    public class Quests : UniversalFileWrapper<Quest>
    {
        public Quests(IMemory game, Func<long> address) : base(game, address) { }

        public IList<Quest> EntriesList
        {
            get
            {
                CheckCache();
                return CachedEntriesList.ToList();
            }
        }

        public Quest GetByAddress(long address) => base.GetByAddress(address);
    }
}