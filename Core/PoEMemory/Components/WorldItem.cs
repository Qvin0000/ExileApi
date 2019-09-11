using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;

namespace ExileCore.PoEMemory.Components
{
    public class WorldItem : Component
    {
        private readonly CachedValue<Entity> _cachedValue;

        public WorldItem()
        {
            _cachedValue = new FrameCache<Entity>(() => Address != 0 ? ReadObject<Entity>(Address + 0x28) : null);
        }

        //Size 0x28
        public Entity ItemEntity => _cachedValue.Value;
    }
}
