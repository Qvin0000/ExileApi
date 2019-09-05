using Exile.PoEMemory.MemoryObjects;

namespace PoEMemory.Components
{
    public class Animated : Component
    {
        public Entity BaseAnimatedObjectEntity => GetObject<Entity>(M.Read<long>(Address + 0x78));
    }
}