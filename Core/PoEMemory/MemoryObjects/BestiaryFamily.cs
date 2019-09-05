namespace PoEMemory
{
    public class BestiaryFamily : RemoteMemoryObject
    {
        public int Id { get; internal set; }

        private string familyId;
        public string FamilyId => familyId != null ? familyId : familyId = M.ReadStringU(M.Read<long>(Address));

        private string name;
        public string Name => name != null ? name : name = M.ReadStringU(M.Read<long>(Address + 0x8));

        public string Icon => M.ReadStringU(M.Read<long>(Address + 0x10));
        public string SmallIcon => M.ReadStringU(M.Read<long>(Address + 0x18));
        public string Illustration => M.ReadStringU(M.Read<long>(Address + 0x20));
        public string PageArt => M.ReadStringU(M.Read<long>(Address + 0x28));
        public string Description => M.ReadStringU(M.Read<long>(Address + 0x30));

        public override string ToString() => Name;
    }
}