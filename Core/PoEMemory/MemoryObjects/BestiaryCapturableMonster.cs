namespace ExileCore.PoEMemory.MemoryObjects
{
    public class BestiaryCapturableMonster : RemoteMemoryObject
    {
        private BestiaryCapturableMonster bestiaryCapturableMonsterKey;
        private BestiaryGenus bestiaryGenus;
        private BestiaryGroup bestiaryGroup;
        private string monsterName;
        private MonsterVariety monsterVariety;
        public int Id { get; set; }
        public string MonsterName => monsterName != null ? monsterName : monsterName = M.ReadStringU(M.Read<long>(Address + 0x20));
        public MonsterVariety MonsterVariety =>
            monsterVariety != null
                ? monsterVariety
                : monsterVariety = TheGame.Files.MonsterVarieties.GetByAddress(M.Read<long>(Address + 0x8));
        public BestiaryGroup BestiaryGroup =>
            bestiaryGroup != null
                ? bestiaryGroup
                : bestiaryGroup = TheGame.Files.BestiaryGroups.GetByAddress(M.Read<long>(Address + 0x18));
        public long BestiaryEncountersPtr => M.Read<long>(Address + 0x30);
        public BestiaryCapturableMonster BestiaryCapturableMonsterKey =>
            bestiaryCapturableMonsterKey != null
                ? bestiaryCapturableMonsterKey
                : bestiaryCapturableMonsterKey =
                    TheGame.Files.BestiaryCapturableMonsters.GetByAddress(M.Read<long>(Address + 0x6a));
        public BestiaryGenus BestiaryGenus =>
            bestiaryGenus != null
                ? bestiaryGenus
                : bestiaryGenus = TheGame.Files.BestiaryGenuses.GetByAddress(M.Read<long>(Address + 0x61));
        public int AmountCaptured => TheGame.IngameState.ServerData.GetBeastCapturedAmount(this);

        public override string ToString()
        {
            return $"Nane: {MonsterName}, Group: {BestiaryGroup.Name}, Family: {BestiaryGroup.Family.Name}, Captured: {AmountCaptured}";
        }
    }
}
