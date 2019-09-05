using System;
using System.Runtime.Serialization;
using Shared.Static;

namespace Shared.Interfaces
{
    public class TimeCache<T> : CachedValue<T>
    {
        private readonly long _waitMilliseconds;
        private long time;

        public TimeCache(Func<T> func, long waitMilliseconds) : base(func) {
            time = long.MinValue;
            _waitMilliseconds = waitMilliseconds;
        }


        protected override bool Update(bool force) {
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