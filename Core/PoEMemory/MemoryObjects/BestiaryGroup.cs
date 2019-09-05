namespace PoEMemory
{
    public class BestiaryGroup : RemoteMemoryObject
    {
        public int Id { get; internal set; }

        private string groupId;
        public string GroupId => groupId != null ? groupId : groupId = M.ReadStringU(M.Read<long>(Address));

        public string Description => M.ReadStringU(M.Read<long>(Address + 0x8));
        public string Illustration => M.ReadStringU(M.Read<long>(Address + 0x10));

        private string name;
        public string Name => name != null ? name : name = M.ReadStringU(M.Read<long>(Address + 0x18));

        public string SmallIcon => M.ReadStringU(M.Read<long>(Address + 0x20));
        public string ItemIcon => M.ReadStringU(M.Read<long>(Address + 0x28));

        private BestiaryFamily family;

        public BestiaryFamily Family =>
            family != null ? family : family = (BestiaryFamily) TheGame.Files.BestiaryFamilies.GetByAddress(M.Read<long>(Address + 0x38));

        public override string ToString() => Name;
    }
}