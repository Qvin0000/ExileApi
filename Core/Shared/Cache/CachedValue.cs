using System;
using System.Diagnostics;
using System.Threading;

namespace ExileCore.Shared.Cache
{
    public abstract class CachedValue
    {
        public static int TotalCount;
        public static int LifeCount;
        public static float Latency { get; set; } = 25;
    }

    public abstract class CachedValue<T> : CachedValue
    {
        public delegate void CacheUpdateEvent(T t);

        protected static Stopwatch sw = Stopwatch.StartNew();
        private readonly Func<T> _func;
        private bool _force;
        private T _value;

        //private object obj;
        protected CachedValue(Func<T> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func), "Cached Value ctor null function");
            Interlocked.Increment(ref TotalCount);
            Interlocked.Increment(ref LifeCount);
        }

        public T Value
        {
            get
            {
                if (Update(_force))
                {
                    //TODO TEST LOCK 
                    // lock (obj)
                    {
                        _force = false;
                        _value = _func();
                    }
                    OnUpdate?.Invoke(_value);
                    _updated = true;
                    return _value;
                }
                else
                {
                    if (!_updated)
                    {
                        return _func();
                    }
                    
                }
                return _value;
            }
        }

        private bool _updated = false;
        public T RealValue => _func();
        public event CacheUpdateEvent OnUpdate;

        public void ForceUpdate()
        {
            _force = true;
        }

        protected abstract bool Update(bool force);

        ~CachedValue()
        {
            Interlocked.Decrement(ref LifeCount);
        }
    }
}
