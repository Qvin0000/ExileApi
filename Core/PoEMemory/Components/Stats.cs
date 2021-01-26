using System;
using System.Collections.Generic;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;

namespace ExileCore.PoEMemory.Components
{
    public class Stats : Component
    {
        private readonly CachedValue<StatsComponentOffsets> _statsValue;
        private readonly CachedValue<SubStatsComponentOffsets> _substructStatsValue;
        private readonly CachedValue<Dictionary<GameStat, int>> _statDictionary;
        private readonly Dictionary<string, int> testHumanDictionary = new Dictionary<string, int>();
        private Dictionary<GameStat, int> testStatDictionary = new Dictionary<GameStat, int>();

        public Stats()
        {
            _statsValue = new FrameCache<StatsComponentOffsets>(() => M.Read<StatsComponentOffsets>(Address));
            _substructStatsValue = new FrameCache<SubStatsComponentOffsets>(() => M.Read<SubStatsComponentOffsets>(_statsValue.Value.SubStatsPtr));
            _statDictionary = new FrameCache<Dictionary<GameStat, int>>(ParseStats);

            // _humanDictionary = new FrameCache<Dictionary<string,int>>(HumanStats);
        }

        public new long OwnerAddress => _statsValue.Value.Owner;
        public SubStatsComponentOffsets StatsComponent => _substructStatsValue.Value;

        //   private CachedValue<Dictionary<string, int>> _humanDictionary;
        //Stats goes as sequence of 2 values, 4 byte each. First goes stat ID then goes stat value
        public Dictionary<GameStat, int> StatDictionary => _statDictionary.Value;

        // public Dictionary<string, int> HumanStatictionary => _humanDictionary.Value;
        public long StatsCount => (StatsComponent.Stats.Last - StatsComponent.Stats.First) / 8;

        public Dictionary<GameStat, int> ParseStats()
        {
            if (Address == 0) return testStatDictionary;

            var statPtrStart = StatsComponent.Stats.First;
            var statPtrLast = StatsComponent.Stats.Last;
            var statPtrEnd = StatsComponent.Stats.End;
            if (StatsComponent.Stats.Size <= 0) return testStatDictionary;
            var key = 0;
            var value = 0;
            var total_stats = statPtrLast - statPtrStart;
            var max_stats = statPtrEnd - statPtrStart;
            if (total_stats > max_stats || statPtrStart == 0) return testStatDictionary;
            var bytes = M.ReadMem(statPtrStart, (int) total_stats);
            var capacity = max_stats / 8;

            if (max_stats > 9000)
            {
                Core.Logger.Error(
                    $"Stats over capped: {StatsComponent.Stats} Total Stats: {total_stats} Max Stats: {max_stats}");

                return testStatDictionary;
            }

            if (capacity < 0) return testStatDictionary;
            if (testStatDictionary.Count < capacity) testStatDictionary = new Dictionary<GameStat, int>((int) capacity);
            testStatDictionary.Clear();

            for (var i = 0; i < bytes.Length - 0x04; i += 8)
            {
                try
                {
                    key = BitConverter.ToInt32(bytes, i);
                    value = BitConverter.ToInt32(bytes, i + 0x04);
                    testStatDictionary[(GameStat) key] = value;
                }
                catch (Exception e)
                {
                    throw new Exception($"Stats parse {e}");
                }
            }

            return testStatDictionary;
        }

        public Dictionary<string, int> HumanStats()
        {
            var dictionary = StatDictionary;
            testHumanDictionary.Clear();

            //  var dict = new Dictionary<string, int>(dictionary.Count);

            var stats = TheGame.Files.Stats;

            if (stats == null)
                return null;

            foreach (var d in dictionary)
            {
                if (stats.recordsById.TryGetValue((int) d.Key, out var res))
                    testHumanDictionary[res.Key] = d.Value;
            }

            return testHumanDictionary;
        }
    }
}
