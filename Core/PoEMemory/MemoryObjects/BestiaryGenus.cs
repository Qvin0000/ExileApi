namespace PoEMemory
{
    public class BestiaryGenus : RemoteMemoryObject
    {
        public int Id { get; internal set; }

        private string genusId;
        public string GenusId => genusId != null ? genusId : genusId = M.ReadStringU(M.Read<long>(Address));

        private string name;
        public string Name => name != null ? name : name = M.ReadStringU(M.Read<long>(Address + 0x8));

        private BestiaryGroup bestiaryGroup;

        public BestiaryGroup BestiaryGroup =>
            bestiaryGroup != null ? bestiaryGroup : bestiaryGroup = TheGame.Files.BestiaryGroups.GetByAddress(M.Read<long>(Address + 0x18));

        private string name2;
        public string Name2 => name2 != null ? name2 : name2 = M.ReadStringU(M.Read<long>(Address + 0x20));

        private string icon;
        public string Icon => icon != null ? icon : icon = M.ReadStringU(M.Read<long>(Address + 0x28));

        public int MaxInStorage => M.Read<int>(Address + 0x30);

        public override string ToString() => $"{Name}, MaxInStorage: {MaxInStorage}";
    }
}