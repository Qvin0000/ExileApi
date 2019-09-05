using System;
using System.Runtime.Serialization;
using Shared.Static;

namespace Shared.Interfaces
{
    public class LatancyCache<T> : CachedValue<T>
    {
        private readonly int _minLatency;
        private long _checkTime;


        public LatancyCache(Func<T> func, int minLatency = 10) : base(func) {
            _minLatency = minLatency;
            _checkTime = long.MinValue;
        }

        protected override bool Update(bool force) {
            var curLatency = Latency;
            var time = sw.ElapsedMilliseconds;

            if (time >= _checkTime || force)
            {
                if (curLatency > _minLatency)
                    _checkTime = (long) (time + curLatency);
                else
                    _checkTime = time + _minLatency;
                return true;
            }

            return false;
        }
    }
}