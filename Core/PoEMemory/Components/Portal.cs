using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Components
{
    public class Portal : Component
    {
        public WorldArea Area => TheGame.Files.WorldAreas.GetByAddress(M.Read<long>(Address + 0x30));
    }
}
