using System;

namespace ExileCore.Shared.Cache
{
    public class TimeCache<T> : CachedValue<T>
    {
        private long _waitMilliseconds;
        private long time;

        public TimeCache(Func<T> func, long waitMilliseconds) : base(func)
        {
            time = long.MinValue;
            _waitMilliseconds = waitMilliseconds;
        }

        public void NewTime(long newTime)
        {
            _waitMilliseconds = newTime;
            time = _waitMilliseconds + sw.ElapsedMilliseconds;
        }
        protected override bool Update(bool force)
        {
            var nowTime = sw.ElapsedMilliseconds;

            if (nowTime >= time || force)
            {
                time = nowTime + _waitMilliseconds;
                return true;
            }

            return false;
        }
    }
}
