using System;

namespace ExileCore.Shared.Cache
{
    public class ConditionalCache<T> : CachedValue<T>
    {
        private readonly Func<bool> _cond;

        public ConditionalCache(Func<T> func, Func<bool> cond) : base(func)
        {
            _cond = cond;
        }

        protected override bool Update(bool force)
        {
            if (_cond() || force) return true;

            return false;
        }
    }
}
