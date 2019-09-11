using System;
using System.Runtime.Caching;
using System.Threading;
using ExileCore.Shared.Interfaces;

namespace ExileCore.Shared.Cache
{
    public class StaticCache<T> : IStaticCache<T>
    {
        private static readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private readonly int _lifeTimeForCache;
        private readonly int _limit;
        private readonly string name;
        private readonly CacheItemPolicy _policy;
        private readonly MemoryCache cache;
        private bool IsEmpty = true;

        public StaticCache(int lifeTimeForCache = 120, int limit = 30, string name = null)
        {
            _lifeTimeForCache = lifeTimeForCache;
            _limit = limit;
            this.name = name ?? typeof(T).Name;
            cache = new MemoryCache(this.name);

            _policy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromSeconds(lifeTimeForCache), RemovedCallback = arguments => { DeletedCache++; }
            };
        }

        public void UpdateCache()
        {
            if (!IsEmpty)
            {
                cache.Trim(100);
                IsEmpty = true;
            }
        }

        public int Count => ReadMemory - DeletedCache;
        public int DeletedCache { get; private set; }
        public int ReadCache { get; private set; }
        public int ReadMemory { get; private set; }
        public string CoeffString => $"{Coeff:0.000}% Read from memory";
        public float Coeff => ReadMemory / (float) (ReadCache + ReadMemory) * 100;

        public T Read(string addr, Func<T> func)
        {
            cacheLock.EnterReadLock();

            try
            {
                IsEmpty = false;
                var o = cache[addr];

                if (o != null)
                {
                    ReadCache++;
                    return (T) o;
                }
            }
            finally
            {
                cacheLock.ExitReadLock();
            }

            cacheLock.EnterUpgradeableReadLock();

            try
            {
                var ob = cache.Get(addr);

                if (ob != null)
                {
                    ReadCache++;
                    return (T) ob;
                }

                try
                {
                    cacheLock.EnterWriteLock();
                    var tt = func();
                    ReadMemory++;
                    cache.Add(addr, tt, _policy);
                    return tt;
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
        }

        public bool Remove(string key)
        {
            var remove = cache.Remove(key);

            if (remove != null)
                return true;

            return false;
        }
    }
}
