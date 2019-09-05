using GameOffsets;
using Shared.Interfaces;

namespace PoEMemory.Components
{
    public class Targetable : Component
    {
        private CachedValue<TargetableComponentOffsets> _cachedValue;

        public TargetableComponentOffsets TargetableComponent => _cachedValue.Value;

        public Targetable() => _cachedValue = new FrameCache<TargetableComponentOffsets>(() => M.Read<TargetableComponentOffsets>(Address));
        public bool isTargetable => TargetableComponent.isTargetable;
        public bool isTargeted => TargetableComponent.isTargeted;
    }
}