using System;
using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.FilesInMemory
{
    public class PassiveSkill : RemoteMemoryObject
    {
        private string id;
        private string name;
        private int passiveId = -1;
        private List<Tuple<StatsDat.StatRecord, int>> stats;
        public int PassiveId => passiveId != -1 ? passiveId : passiveId = M.Read<int>(Address + 0x30);
        public string Id => id != null ? id : id = M.ReadStringU(M.Read<long>(Address), 255);
        public string Name => name != null ? name : name = M.ReadStringU(M.Read<long>(Address + 0x34), 255);
        public string Icon => M.ReadStringU(M.Read<long>(Address + 0x8), 255); //Read on request

        public IEnumerable<Tuple<StatsDat.StatRecord, int>> Stats
        {
            get
            {
                if (stats == null)
                {
                    stats = new List<Tuple<StatsDat.StatRecord, int>>();

                    var statsCount = M.Read<int>(Address + 0x10);
                    var pointerToStats = M.Read<long>(Address + 0x18);
                    var statsPointers = M.ReadSecondPointerArray_Count(pointerToStats, statsCount);

                    stats = statsPointers.Select((x, i) =>
                        new Tuple<StatsDat.StatRecord, int>(
                            TheGame.Files.Stats.GetStatByAddress(x), ReadStatValue(i))).ToList();
                }

                return stats;
            }
        }

        internal int ReadStatValue(int index)
        {
            return M.Read<int>(Address + 0x20 + index * 4);
        }

        public override string ToString()
        {
            return $"{Name}, Id: {Id}, PassiveId: {PassiveId}";
        }
    }
}
