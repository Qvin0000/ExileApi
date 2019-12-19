using ExileCore.PoEMemory.Models;

namespace ExileCore.PoEMemory.FilesInMemory.Metamorph
{
    public class MetamorphMetaSkillType : RemoteMemoryObject
    {
        public string Id => M.ReadStringU(M.Read<long>(Address + 0x0), 255);
        public string Name => M.ReadStringU(M.Read<long>(Address + 0x8), 255);
        public string Description => M.ReadStringU(M.Read<long>(Address + 0x10), 255);
        public BaseItemType BaseItemType => TheGame.Files.BaseItemTypes.GetFromAddress(M.Read<long>(Address + 0x38));
        public string BodyPart => M.ReadStringU(M.Read<long>(Address + 0x40), 255);
        public override string ToString()
        {
            return $"{BodyPart}, {Id}, {Name}, {BaseItemType?.BaseName}, {Description}";
        }
    }
}