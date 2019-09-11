using System;

namespace ExileCore.Shared.Cache
{
    public class StaticValueCache<T> : CachedValue<T>
    {
        private bool first;

        public StaticValueCache(Func<T> func) : base(func)
        {
            first = true;
        }

        protected override bool Update(bool force)
        {
            if (first)
            {
                first = false;
                return true;
            }

            return false;
        }
    }
}
