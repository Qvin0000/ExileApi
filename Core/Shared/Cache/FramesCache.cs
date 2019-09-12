using System;

namespace ExileCore.Shared.Cache
{
    public class FramesCache<T> : FrameCache<T>
    {
        private readonly uint _waitFrames;
        private uint _frame;

        public FramesCache(Func<T> func, uint waitFrames = 1) : base(func)
        {
            _waitFrames = waitFrames;
            _frame = uint.MinValue;
        }

        protected override bool Update(bool force)
        {
            if (Core.FramesCount >= _frame || force)
            {
                _frame += _waitFrames;
                return true;
            }

            return false;
        }
    }
}
