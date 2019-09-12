using System;

namespace ExileCore.Shared.Cache
{
    public class FrameCache<T> : CachedValue<T>
    {
        private uint _frame;

        public FrameCache(Func<T> func) : base(func)
        {
            _frame = uint.MaxValue;
        }

        protected override bool Update(bool force)
        {
            if (_frame != Core.FramesCount || force)
            {
                _frame = Core.FramesCount;
                return true;
            }

            return false;
        }
    }
}
