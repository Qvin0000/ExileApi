namespace PoEMemory
{
    public class ActorVaalSkill : RemoteMemoryObject
    {
        private const int NAMES_POINTER_OFFSET = 0x8;
        private const int INTERNAL_NAME_OFFSET = 0x0;
        private const int NAME_OFFSET = 0x8;
        private const int DESCRIPTION_OFFSET = 0x10;
        private const int SKILL_NAME_OFFSET = 0x18;
        private const int ICON_OFFSET = 0x20;

        private const int MAX_VAAL_SOULS_OFFSET = 0x10;
        private const int VAAL_SOULS_PER_USE_OFFSET = 0x14;
        private const int CURRENT_VAAL_SOULS_OFFSET = 0x18;

        public string VaalSkillInternalName => M.ReadStringU(M.Read<long>(NAMES_POINTER_OFFSET) + INTERNAL_NAME_OFFSET);
        public string VaalSkillDisplayName => M.ReadStringU(M.Read<long>(NAMES_POINTER_OFFSET) + NAME_OFFSET);
        public string VaalSkillDescription => M.ReadStringU(M.Read<long>(NAMES_POINTER_OFFSET) + DESCRIPTION_OFFSET);
        public string VaalSkillSkillName => M.ReadStringU(M.Read<long>(NAMES_POINTER_OFFSET) + SKILL_NAME_OFFSET);
        public string VaalSkillIcon => M.ReadStringU(M.Read<long>(NAMES_POINTER_OFFSET) + ICON_OFFSET);

        public int VaalMaxSouls => M.Read<int>(Address + MAX_VAAL_SOULS_OFFSET);
        public int VaalSoulsPerUse => M.Read<int>(Address + VAAL_SOULS_PER_USE_OFFSET);
        public int CurrVaalSouls => M.Read<int>(Address + CURRENT_VAAL_SOULS_OFFSET);
    }
}