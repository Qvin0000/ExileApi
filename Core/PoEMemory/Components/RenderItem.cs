using System;

namespace PoEMemory.Components
{
    public class RenderItem : Component
    {
        public string ResourcePath =>
            Cache.StringCache.Read($"{nameof(RenderItem)}{Address + 0x20}", () => M.ReadStringU(M.Read<long>(Address + 0x20)));
    }
}