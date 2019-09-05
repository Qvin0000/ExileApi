using System;
using Shared.Interfaces;


namespace PoEMemory.FilesInMemory
{
    public class BestiaryCapturableMonsters : UniversalFileWrapper<BestiaryCapturableMonster>
    {
        public BestiaryCapturableMonsters(IMemory m, Func<long> address) : base(m, address) { }

        private int IdCounter;
        protected void EntryAdded(long addr, BestiaryCapturableMonster entry) => entry.Id = IdCounter++;
    }
}