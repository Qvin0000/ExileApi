using Exile.PoEMemory.MemoryObjects;
using Shared.Interfaces;

namespace PoEMemory.Components
{
    public class WorldItem : Component
    {
        private CachedValue<Entity> _cachedValue;
        public WorldItem() => _cachedValue = new FrameCache<Entity>(() => Address != 0 ? ReadObject<Entity>(Address + 0x28) : null);

        //Size 0x28
        public Entity ItemEntity => _cachedValue.Value;
    }
}