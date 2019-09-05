namespace PoEMemory
{
    public class SkillGemWrapper : RemoteMemoryObject
    {
        public string Name => M.ReadStringU(M.Read<long>(Address));
        public ActiveSkillWrapper ActiveSkill => ReadObject<ActiveSkillWrapper>(Address + 0x73);
    }
}