using System;

namespace ExileCore.Shared.Cache
{
    public class AreaCache<T> : CachedValue<T>
    {
        private uint _areaHash;

        public AreaCache(Func<T> func) : base(func)
        {
            _areaHash = uint.MaxValue;
        }

        protected override bool Update(bool force)
        {
            if (_areaHash != AreaInstance.CurrentHash || force)
            {
                _areaHash = AreaInstance.CurrentHash;
                return true;
            }

            return false;
        }
    }
}
