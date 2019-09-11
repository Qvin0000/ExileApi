using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.Components
{
    public class AreaTransition : Component
    {
        public int WorldAreaId => M.Read<ushort>(Address + 0x28);
        public WorldArea AreaById => TheGame.Files.WorldAreas.GetAreaByAreaId(WorldAreaId);
        public WorldArea WorldArea => TheGame.Files.WorldAreas.GetByAddress(M.Read<long>(Address + 0x48));
        public AreaTransitionType TransitionType => (AreaTransitionType) M.Read<byte>(Address + 0x2A);
    }
}
