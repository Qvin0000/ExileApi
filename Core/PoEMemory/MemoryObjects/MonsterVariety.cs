using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class MonsterVariety : RemoteMemoryObject
    {
        private string varietyId;
        public int Id { get; internal set; }
        public string VarietyId => varietyId != null ? varietyId : varietyId = M.ReadStringU(M.Read<long>(Address));
        public long MonsterTypePtr => M.Read<long>(Address + 0x10); //TODO
        public int ObjectSize => M.Read<int>(Address + 0x1c);
        public int MinimumAttackDistance => M.Read<int>(Address + 0x20);
        public int MaximumAttackDistance => M.Read<int>(Address + 0x24);
        public string ACTFile => M.ReadStringU(M.Read<long>(Address + 0x28));
        public string AOFile => M.ReadStringU(M.Read<long>(Address + 0x30));
        public string BaseMonsterTypeIndex => M.ReadStringU(M.Read<long>(Address + 0x38));

        public IEnumerable<ModsDat.ModRecord> Mods
        {
            get
            {
                var count = M.Read<int>(Address + 0x40);
                var pointers = M.ReadSecondPointerArray_Count(M.Read<long>(Address + 0x48), count);

                return pointers.Select(x => TheGame.Files.Mods.GetModByAddress(x)).ToList();
            }
        }

        public int ModelSizeMultiplier => M.Read<int>(Address + 0x64);
        public int ExperienceMultiplier => M.Read<int>(Address + 0x8c);
        public int CriticalStrikeChance => M.Read<int>(Address + 0xac);

        //public int GrantedEffectsCount => M.Read<int>(Address + 0xb4);
        //public long GrantedEffectsPtr => M.Read<long>(Address + 0xbc);
        public string AISFile => M.ReadStringU(M.Read<long>(Address + 0xc4));

        //public int ModKeysCount => M.Read<int>(Address + 0xcc);
        //public long ModKeysPtr => M.Read<long>(Address + 0xd4);
        public string MonsterName => M.ReadStringU(M.Read<long>(Address + 0x104));
        public int DamageMultiplier => M.Read<int>(Address + 0xfc);
        public int LifeMultiplier => M.Read<int>(Address + 0x100);
        public int AttackSpeed => M.Read<int>(Address + 0x104);

        public override string ToString()
        {
            return $"Name: {MonsterName}, VarietyId: {VarietyId}, BaseMonsterTypeIndex: {BaseMonsterTypeIndex}";
        }
    }
}
