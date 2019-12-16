using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.FilesInMemory
{
    public class MetamorphosisMetaSkill : RemoteMemoryObject
    {
        public MonsterVariety MonsterVarietyMetadata => TheGame.Files.MonsterVarieties.GetByAddress(M.Read<long>(Address + 0x8));
        public MetamorphosisMetaSkillType MetaSkill => TheGame.Files.MetamorphosisMetaSkillTypes.GetByAddress(M.Read<long>(Address + 0x18));
        public string SkillName => M.ReadStringU(M.Read<long>(Address + 0xE8), 255);
        public override string ToString()
        {
            return $"{MetaSkill}, {MonsterVarietyMetadata?.VarietyId}";
        }
    }
}
