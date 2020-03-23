using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class AreaTemplate : RemoteMemoryObject
    {
        public string RawName => M.ReadStringU(M.Read<long>(Address));
        public string Name => M.ReadStringU(M.Read<long>(Address + 8));
        public int Act => M.Read<int>(Address + 0x10);
        public bool IsTown => M.Read<byte>(Address + 0x14) == 1;
        public bool HasWaypoint => M.Read<byte>(Address + 0x15) == 1;
        public int NominalLevel => M.Read<int>(Address + 0x26); //Not sure
        public int MonsterLevel => M.Read<int>(Address + 0x26);
        public int WorldAreaId => M.Read<int>(Address + 0x2A);
        public int CorruptedAreasVariety => M.Read<int>(Address + 0xFB);
        public List<WorldArea> PossibleCorruptedAreas => _PossibleCorruptedAreas(Address + 0x103, CorruptedAreasVariety);

        private List<WorldArea> _PossibleCorruptedAreas(long address, int count)
        {
            var list = new List<WorldArea>();

            for (var i = 0; i < count; i++)
            {
                list.Add(GetObject<WorldArea>(address + i * 8));
            }

            return list;
        }
    }
}
