using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class Cursor : Element
    {
        private readonly CachedValue<CursorOffsets> _cachevalue;

        public Cursor()
        {
            _cachevalue = new FrameCache<CursorOffsets>(() => M.Read<CursorOffsets>(Address));
        }

        public MouseActionType Action => (MouseActionType) M.Read<int>(Address + 0x238);
        public MouseActionType ActionCached => (MouseActionType) _cachevalue.Value.Action;
        public int ClicksCached => _cachevalue.Value.Clicks;
        public int Clicks => M.Read<int>(Address + 0x24C);
        public string ActionString => M.ReadNativeString(Address + 0x2A0);
        public string ActionStringCached => _cachevalue.Value.ActionString.ToString(M);
    }
}
