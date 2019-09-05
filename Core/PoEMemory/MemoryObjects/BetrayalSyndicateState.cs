using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoEMemory.FilesInMemory;

namespace PoEMemory.MemoryObjects
{
    public class BetrayalSyndicateState : RemoteMemoryObject
    {
        public static int STRUCT_SIZE = 0x98;

        public Element UIElement => ReadObjectAt<Element>(0);

        public float PosX => M.Read<float>(Address + 0x7C);
        public float PosY => M.Read<float>(Address + 0x7C);

        public BetrayalTarget Target => TheGame.Files.BetrayalTargets.GetByAddress(M.Read<long>(Address + 0x10));
        public BetrayalJob Job => TheGame.Files.BetrayalJobs.GetByAddress(M.Read<long>(Address + 0x20));
        public BetrayalRank Rank => TheGame.Files.BetrayalRanks.GetByAddress(M.Read<long>(Address + 0x30));

        public BetrayalReward Reward =>
            TheGame.Files.BetrayalRewards.EntriesList.Find(x => x.Target == Target && x.Job == Job && x.Rank == Rank);

        public List<BetrayalUpgrade> BetrayalUpgrades
        {
            get
            {
                var startAddress = M.Read<long>(Address + 0x38);
                var endAddress = M.Read<long>(Address + 0x40);
                var result = new List<BetrayalUpgrade>();

                for (var addr = startAddress; addr < endAddress; addr += 0x10) result.Add(ReadObject<BetrayalUpgrade>(addr + 0x8));

                return result;
            }
        }

        public List<BetrayalSyndicateState> Relations
        {
            get
            {
                var relationAddress = M.Read<long>(Address + 0x50);
                var result = new List<BetrayalSyndicateState>();
                for (var i = 0; i < 3; i++)
                {
                    var address = M.Read<long>(relationAddress + i * 0x8);
                    if (address != 0) result.Add(GetObject<BetrayalSyndicateState>(address));
                }

                return result;
            }
        }


        public override string ToString() => $"{Target?.Name}, {Rank?.Name}, {Job?.Name}";
    }

    public class BetrayalUpgrade : RemoteMemoryObject
    {
        public string UpgradeName => M.ReadStringU(M.Read<long>(Address + 0x8));
        public string UpgradeStat => M.ReadStringU(M.Read<long>(Address + 0x10));
        public string Art => M.ReadStringU(M.Read<long>(Address + 0x28));

        public override string ToString() => $"{UpgradeName} ({UpgradeStat})";
    }
}