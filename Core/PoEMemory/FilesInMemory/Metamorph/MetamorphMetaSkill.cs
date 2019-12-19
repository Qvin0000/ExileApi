using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.FilesInMemory.Metamorph
{
    public class MetamorphMetaSkill : RemoteMemoryObject
    {
        public MonsterVariety MonsterVarietyMetadata => TheGame.Files.MonsterVarieties.GetByAddress(M.Read<long>(Address + 0x8));
        public MetamorphMetaSkillType MetaSkill => TheGame.Files.MetamorphMetaSkillTypes.GetByAddress(M.Read<long>(Address + 0x18));
        public string SkillName => M.ReadStringU(M.Read<long>(Address + 0xE8), 255);
        public string GrantedEffect1 => M.ReadStringU(M.Read<long>(Address + 0x28, 0), 255);
        public string GrantedEffect2 => M.ReadStringU(M.Read<long>(Address + 0x58, 0), 255);
        public override string ToString()
        {
            return $"{MetaSkill}, {MonsterVarietyMetadata?.VarietyId}";
        }
    }
}
