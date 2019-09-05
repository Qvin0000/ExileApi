using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Interfaces;

namespace PoEMemory.FilesInMemory
{
    public class PassiveSkills : UniversalFileWrapper<PassiveSkill>
    {
        public Dictionary<int, PassiveSkill> PassiveSkillsDictionary { get; } = new Dictionary<int, PassiveSkill>();

        private bool loaded;
        public PassiveSkills(IMemory m, Func<long> address) : base(m, address) { }

        public PassiveSkill GetPassiveSkillByPassiveId(int index) {
            CheckCache();

            if (!loaded)
            {
                foreach (var passiveSkill in EntriesList) EntryAdded(passiveSkill.Address, passiveSkill);
                loaded = true;
            }

            PassiveSkillsDictionary.TryGetValue(index, out var result);
            return result;
        }

        public PassiveSkill GetPassiveSkillById(string id) => EntriesList.FirstOrDefault(x => x.Id == id);

        protected void EntryAdded(long addr, PassiveSkill entry) => PassiveSkillsDictionary.Add(entry.PassiveId, entry);

        private List<PassiveSkill> _EntriesList = null;
        public IList<PassiveSkill> EntriesList => _EntriesList ?? (_EntriesList = base.EntriesList.ToList());
        public PassiveSkill GetByAddress(long address) => base.GetByAddress(address);
    }
}