using System;
using System.Collections.Generic;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class IngameData : RemoteMemoryObject
    {
        private readonly CachedValue<IngameDataOffsets> _cacheStruct;
        private readonly CachedValue<AreaTemplate> _CurrentArea;
        private readonly CachedValue<WorldArea> _CurrentWorldArea;
        private readonly CachedValue<long> _EntitiesCount;
        private EntityList _EntityList;
        private readonly CachedValue<Entity> _localPlayer;
        private NativePtrArray cacheMapStats;
        private NativePtrArray cacheStats;
        private readonly Dictionary<GameStat, int> mapStats = new Dictionary<GameStat, int>();

        public IngameData()
        {
            _cacheStruct = new AreaCache<IngameDataOffsets>(() => M.Read<IngameDataOffsets>(Address));
            _localPlayer = new AreaCache<Entity>(() => GetObject<Entity>(_cacheStruct.Value.LocalPlayer));
            _CurrentArea = new AreaCache<AreaTemplate>(() => GetObject<AreaTemplate>(_cacheStruct.Value.CurrentArea));
            _CurrentWorldArea = new AreaCache<WorldArea>(() => TheGame.Files.WorldAreas.GetByAddress(CurrentArea.Address));
            var offset = Extensions.GetOffset<IngameDataOffsets>(nameof(IngameDataOffsets.EntitiesCount));
            _EntitiesCount = new FrameCache<long>(() => M.Read<long>(Address + offset));
        }

        public IngameDataOffsets DataStruct => _cacheStruct.Value;
        public long EntitiesCount => _EntitiesCount.Value;
        public AreaTemplate CurrentArea => _CurrentArea.Value;
        public WorldArea CurrentWorldArea => _CurrentWorldArea.Value;
        public int CurrentAreaLevel => _cacheStruct.Value.CurrentAreaLevel;
        public uint CurrentAreaHash => _cacheStruct.Value.CurrentAreaHash;
        public Entity LocalPlayer => _localPlayer.Value;
        public long EntiteisTest => DataStruct.EntityList;
        public EntityList EntityList => _EntityList ?? (_EntityList = GetObject<EntityList>(DataStruct.EntityList));
        private long LabDataPtr => _cacheStruct.Value.LabDataPtr;
        public LabyrinthData LabyrinthData => LabDataPtr == 0 ? null : GetObject<LabyrinthData>(LabDataPtr);

        public Dictionary<GameStat, int> MapStats
        {
            get
            {
                if (cacheStats.Equals(_cacheStruct.Value.MapStats)) return mapStats;
                mapStats.Clear();
                var statPtrStart = _cacheStruct.Value.MapStats.First;
                var statPtrEnd = _cacheStruct.Value.MapStats.Last;
                var key = 0;
                var value = 0;
                var total_stats = (int) (statPtrEnd - statPtrStart);

                if (total_stats / 8 > 200)
                    return null;

                var bytes = M.ReadMem(statPtrStart, total_stats);

                for (var i = 0; i < bytes.Length; i += 8)
                {
                    key = BitConverter.ToInt32(bytes, i);
                    value = BitConverter.ToInt32(bytes, i + 0x04);
                    mapStats[(GameStat) key] = value;
                }

                cacheStats = _cacheStruct.Value.MapStats;
                return mapStats;
            }
        }

        public IList<PortalObject> TownPortals
        {
            get
            {
                var statPtrStart = M.Read<long>(Address + 0x4B4);
                var statPtrEnd = M.Read<long>(Address + 0x4BC);

                return M.ReadStructsArray<PortalObject>(statPtrStart, statPtrEnd, PortalObject.StructSize, TheGame);
            }
        }

        public class PortalObject : RemoteMemoryObject
        {
            public const int StructSize = 0x38;
            public string PlayerOwner => NativeStringReader.ReadString(Address + 0x08, M);
            public WorldArea Area => TheGame.Files.WorldAreas.GetAreaByAreaId(M.Read<int>(Address + 0x50));

            public override string ToString()
            {
                return $"{PlayerOwner} => {Area.Name}";
            }
        }
    }
}
