using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components
{
    public class Chest : Component
    {
        private readonly CachedValue<ChestComponentOffsets> _cachedValue;
        private readonly CachedValue<StrongboxChestComponentData> _cachedValueStrongboxData;

        public Chest()
        {
            _cachedValue = new FramesCache<ChestComponentOffsets>(() => M.Read<ChestComponentOffsets>(Address), 3);

            _cachedValueStrongboxData =
                new FramesCache<StrongboxChestComponentData>(() => M.Read<StrongboxChestComponentData>(_cachedValue.Value.StrongboxData),
                    3);
        }

        public bool IsOpened => Address != 0 && _cachedValue.Value.IsOpened;
        public bool IsLocked => Address != 0 && _cachedValue.Value.IsLocked;
        public bool IsStrongbox => Address != 0 && _cachedValue.Value.IsStrongbox;
        private long StrongboxData => _cachedValue.Value.StrongboxData;
        public bool DestroyingAfterOpen => Address != 0 && _cachedValueStrongboxData.Value.DestroyingAfterOpen;
        public bool IsLarge => Address != 0 && _cachedValueStrongboxData.Value.IsLarge;
        public bool Stompable => Address != 0 && _cachedValueStrongboxData.Value.Stompable;
        public bool OpenOnDamage => Address != 0 && _cachedValueStrongboxData.Value.OpenOnDamage;
    }
}
