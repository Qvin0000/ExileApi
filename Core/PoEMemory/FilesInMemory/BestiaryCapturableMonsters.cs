using System;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory
{
    public class BestiaryCapturableMonsters : UniversalFileWrapper<BestiaryCapturableMonster>
    {
        private int IdCounter;

        public BestiaryCapturableMonsters(IMemory m, Func<long> address) : base(m, address)
        {
        }

        protected void EntryAdded(long addr, BestiaryCapturableMonster entry)
        {
            entry.Id = IdCounter++;
        }
    }
}
