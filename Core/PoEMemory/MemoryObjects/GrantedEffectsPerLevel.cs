using System;
using System.Collections.Generic;
using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class GrantedEffectsPerLevel : RemoteMemoryObject
    {
        public SkillGemWrapper SkillGemWrapper => ReadObject<SkillGemWrapper>(Address + 0x8);
        public int Level => M.Read<int>(Address + 0x10);
        public int RequiredLevel => M.Read<int>(Address + 0x74);
        public int ManaMultiplier => M.Read<int>(Address + 0x78);

        //public int RequirementsComparer => M.Read<int>(Address + 0x80);
        public int ManaCost => M.Read<int>(Address + 0xa8);
        public int EffectivenessOfAddedDamage => M.Read<int>(Address + 0xac);
        public int Cooldown => M.Read<int>(Address + 0xb4);

        public IEnumerable<Tuple<StatsDat.StatRecord, int>> Stats
        {
            get
            {
                var result = new List<Tuple<StatsDat.StatRecord, int>>();

                var statsCount = M.Read<int>(Address + 0x14);
                var pointerToStats = M.Read<long>(Address + 0x1c);
                pointerToStats += 8;

                for (var i = 0; i < statsCount; i++)
                {
                    var datPtr = M.Read<long>(pointerToStats);
                    var stat = TheGame.Files.Stats.GetStatByAddress(datPtr);
                    result.Add(new Tuple<StatsDat.StatRecord, int>(stat, ReadStatValue(i)));
                    pointerToStats += 16; //16 because we are reading each second pointer
                }

                return result;
            }
        }

        public IEnumerable<string> Tags
        {
            get
            {
                var result = new List<string>();

                var tagsCount = M.Read<int>(Address + 0x44);
                var pointerToTags = M.Read<long>(Address + 0x4c);
                pointerToTags += 8;

                for (var i = 0; i < tagsCount; i++)
                {
                    var tagStringPtr = M.Read<long>(pointerToTags);
                    tagStringPtr = M.Read<long>(tagStringPtr);
                    result.Add(M.ReadStringU(tagStringPtr));
                    pointerToTags += 16; //16 because we are reading each second pointer
                }

                return result;
            }
        }

        public IEnumerable<Tuple<StatsDat.StatRecord, int>> QualityStats
        {
            get
            {
                var result = new List<Tuple<StatsDat.StatRecord, int>>();

                var statsCount = M.Read<int>(Address + 0x84);
                var pointerToStats = M.Read<long>(Address + 0x8c);
                pointerToStats += 8; //Skip first

                for (var i = 0; i < statsCount; i++)
                {
                    var datPtr = M.Read<long>(pointerToStats);
                    var stat = TheGame.Files.Stats.GetStatByAddress(datPtr);
                    result.Add(new Tuple<StatsDat.StatRecord, int>(stat, ReadQualityStatValue(i)));
                    pointerToStats += 16; //16 because we are reading each second pointer
                }

                return result;
            }
        }

        public IEnumerable<StatsDat.StatRecord> TypeStats
        {
            get
            {
                var result = new List<StatsDat.StatRecord>();

                var statsCount = M.Read<int>(Address + 0xbc);
                var pointerToStats = M.Read<long>(Address + 0xc4);
                pointerToStats += 8; //Skip first

                for (var i = 0; i < statsCount; i++)
                {
                    var datPtr = M.Read<long>(pointerToStats);
                    var stat = TheGame.Files.Stats.GetStatByAddress(datPtr);
                    result.Add(stat);
                    pointerToStats += 16; //16 because we are reading each second pointer
                }

                return result;
            }
        }

        internal int ReadStatValue(int index)
        {
            return M.Read<int>(Address + 0x54 + index * 4);
        }

        internal int ReadQualityStatValue(int index)
        {
            return M.Read<int>(Address + 0x9c + index * 4);
        }
    }
}
