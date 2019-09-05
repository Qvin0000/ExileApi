namespace PoEMemory
{
    public class BestiaryCapturableMonster : RemoteMemoryObject
    {
        public int Id { get; set; }

        private string monsterName;
        public string MonsterName => monsterName != null ? monsterName : monsterName = M.ReadStringU(M.Read<long>(Address + 0x20));

        private MonsterVariety monsterVariety;

        public MonsterVariety MonsterVariety =>
            monsterVariety != null
                ? monsterVariety
                : monsterVariety = (MonsterVariety) TheGame.Files.MonsterVarieties.GetByAddress(M.Read<long>(Address + 0x8));

        private BestiaryGroup bestiaryGroup;

        public BestiaryGroup BestiaryGroup =>
            bestiaryGroup != null
                ? bestiaryGroup
                : bestiaryGroup = (BestiaryGroup) TheGame.Files.BestiaryGroups.GetByAddress(M.Read<long>(Address + 0x18));

        public long BestiaryEncountersPtr => M.Read<long>(Address + 0x30);

        private BestiaryCapturableMonster bestiaryCapturableMonsterKey;

        public BestiaryCapturableMonster BestiaryCapturableMonsterKey =>
            bestiaryCapturableMonsterKey != null
                ? bestiaryCapturableMonsterKey
                : bestiaryCapturableMonsterKey =
                    (BestiaryCapturableMonster) TheGame.Files.BestiaryCapturableMonsters.GetByAddress(M.Read<long>(Address + 0x6a));

        private BestiaryGenus bestiaryGenus;

        public BestiaryGenus BestiaryGenus =>
            bestiaryGenus != null
                ? bestiaryGenus
                : bestiaryGenus = (BestiaryGenus) TheGame.Files.BestiaryGenuses.GetByAddress(M.Read<long>(Address + 0x61));

        public int AmountCaptured => TheGame.IngameState.ServerData.GetBeastCapturedAmount(this);

        public override string ToString() =>
            $"Nane: {MonsterName}, Group: {BestiaryGroup.Name}, Family: {BestiaryGroup.Family.Name}, Captured: {AmountCaptured}";
    }
}