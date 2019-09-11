using ExileCore.Shared.Cache;
using ExileCore.Shared.Helpers;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components
{
    public class MinimapIcon : Component
    {
        private FrameCache<MinimapIconOffsets> cachedValue;
        private MinimapIconOffsets MinimapIconOffsets => cachedValue.Value;
        public bool IsVisible => MinimapIconOffsets.IsVisible == 0; //M.Read<byte>(Address + 0x30) == 0;
        public bool IsHide => MinimapIconOffsets.IsHide == 1; //M.Read<byte>(Address + 0x34) == 1;
        private NativeStringU Test => M.Read<NativeStringU>(MinimapIconOffsets.NamePtr); //M.Read<NativeStringU>(Address + 0x28, 0);
        public string TestString => Test.ToString(M);
        public string Name => Cache.StringCache.Read($"{MinimapIconOffsets.NamePtr}{Address}", () => TestString);

        protected override void OnAddressChange()
        {
            cachedValue = new FrameCache<MinimapIconOffsets>(() => M.Read<MinimapIconOffsets>(Address));
        }
    }
}
