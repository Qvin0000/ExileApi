using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components
{
    public class Targetable : Component
    {
        private readonly CachedValue<TargetableComponentOffsets> _cachedValue;

        public Targetable()
        {
            _cachedValue = new FrameCache<TargetableComponentOffsets>(() => M.Read<TargetableComponentOffsets>(Address));
        }

        public TargetableComponentOffsets TargetableComponent => _cachedValue.Value;
        public bool isTargetable => TargetableComponent.isTargetable;
        public bool isTargeted => TargetableComponent.isTargeted;
    }
}
