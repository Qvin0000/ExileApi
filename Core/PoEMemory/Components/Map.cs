using System;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;

namespace ExileCore.PoEMemory.Components
{
    public class Map : Component
    {
        private readonly Lazy<MapComponentBase> mapBase;
        private readonly Lazy<MapComponentInner> mapInner;
        private readonly CachedValue<WorldArea> _area;

        public Map()
        {
            mapBase = new Lazy<MapComponentBase>(() => M.Read<MapComponentBase>(Address));
            mapInner = new Lazy<MapComponentInner>(() => M.Read<MapComponentInner>(mapBase.Value.Base));
            _area = new StaticValueCache<WorldArea>(() => TheGame.Files.WorldAreas.GetByAddress(MapInformation.Area));
        }

        public MapComponentInner MapInformation => mapInner.Value;

        //  public WorldArea Area => Global.Instance.WorldAreasGetByAddress(M.Read<long>(Address + 0x10, 0x18));
        public WorldArea Area => _area.Value;

        //   public int Tier => M.Read<int>(Address + 0x10, 0x90);
        public byte Tier => MapInformation.Tier;

        //   public InventoryTabMapSeries MapSeries => (InventoryTabMapSeries)M.Read<byte>(Address + 0x10, 0x9c);
        public InventoryTabMapSeries MapSeries => (InventoryTabMapSeries) MapInformation.MapSeries;
    }
}
