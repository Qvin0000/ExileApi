using System;
using System.Diagnostics;
using System.Threading;
using Exile;

namespace Shared.Interfaces
{

    public abstract class CachedValue
    {
        public static float Latency { get; set; } = 25;
        public static int TotalCount=0;
        public static int LifeCount=0;
    }
    public abstract class CachedValue<T>:CachedValue
    {


        protected static Stopwatch sw = Stopwatch.StartNew();
        private readonly Func<T> _func;
        private T _value;

        public delegate void CacheUpdateEvent(T t);

        public event CacheUpdateEvent OnUpdate;

        //private object obj;
        public CachedValue(Func<T> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func), "Cached Value ctor null function");
            Interlocked.Increment(ref TotalCount);
            Interlocked.Increment(ref LifeCount);
        }

        public static long GetFromCache = 0;
        public static long ReadingFromMemory = 0;


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
                    OnUpdate?.Invoke(_value);  ReadingFromMemory++;
                }
                else
                {
                    GetFromCache++;
                }
                return _value;
            }
        }

        public T RealValue => _func();

        public void ForceUpdate() => _force = true;
        private bool _force;
        protected abstract bool Update(bool force);

        ~CachedValue()
        {
            Interlocked.Decrement(ref LifeCount);
        }
    }
}