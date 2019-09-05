using System.Collections.Generic;
using PoEMemory;
using PoEMemory.FilesInMemory;
using PoEMemory.MemoryObjects;

namespace Exile.PoEMemory.MemoryObjects
{
    public class BetrayalDialogue : RemoteMemoryObject
    {
        public BetrayalTarget Target => TheGame.Files.BetrayalTargets.GetByAddress(M.Read<long>(Address + 0x8));
        public int Unknown1 => M.Read<int>(Address + 0x10);
        public int Unknown2 => M.Read<int>(Address + 0x14);
        public int Unknown3 => M.Read<int>(Address + 0x38);
        public bool Unknown4 => M.Read<byte>(Address + 0x6c) > 0;
        public bool Unknown5 => M.Read<byte>(Address + 0x8d) > 0;
        public BetrayalJob Job => TheGame.Files.BetrayalJobs.GetByAddress(M.Read<long>(Address + 0x44));
        public BetrayalUpgrade Upgrade => ReadObjectAt<BetrayalUpgrade>(0x64);
        public string DialogueText => M.ReadStringU(M.Read<long>(Address + 0xA6, 0x18));

        public List<int> Keys1 => ReadKeys(0x20);
        public List<int> Keys2 => ReadKeys(0x54);
        public List<int> Keys3 => ReadKeys(0x85);

        private List<int> ReadKeys(long offset) {
            var addr = M.Read<long>(Address + offset);
            var result = new List<int>();
            if (addr != 0)
            {
                for (long i = 0; i < 5; i++) result.Add(M.Read<int>(addr + i * 0x8));
            }

            return result;
        }

        public override string ToString() => $"{Target?.Name}, {Job?.Name}, {Upgrade?.UpgradeName}, {DialogueText}";
    }
}