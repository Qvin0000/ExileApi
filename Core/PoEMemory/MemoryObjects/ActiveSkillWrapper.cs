using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class ActiveSkillWrapper : RemoteMemoryObject
    {
        public string InternalName => M.ReadStringU(M.Read<long>(Address));
        public string DisplayName => M.ReadStringU(M.Read<long>(Address + 0x8));
        public string Description => M.ReadStringU(M.Read<long>(Address + 0x10));
        public string SkillName => M.ReadStringU(M.Read<long>(Address + 0x18));
        public string Icon => M.ReadStringU(M.Read<long>(Address + 0x20));

        public List<int> CastTypes
        {
            get
            {
                var result = new List<int>();
                var castTypesCount = M.Read<int>(Address + 0x28);
                var readAddr = M.Read<long>(Address + 0x30);

                for (var i = 0; i < castTypesCount; i++)
                {
                    result.Add(M.Read<int>(readAddr));
                    readAddr += 4;
                }

                return result;
            }
        }

        public List<int> SkillTypes
        {
            get
            {
                var result = new List<int>();
                var skillTypesCount = M.Read<int>(Address + 0x38);
                var readAddr = M.Read<long>(Address + 0x40);

                for (var i = 0; i < skillTypesCount; i++)
                {
                    result.Add(M.Read<int>(readAddr));
                    readAddr += 4;
                }

                return result;
            }
        }

        public string LongDescription => M.ReadStringU(M.Read<long>(Address + 0x50));
        public string AmazonLink => M.ReadStringU(M.Read<long>(Address + 0x60));
    }
}
