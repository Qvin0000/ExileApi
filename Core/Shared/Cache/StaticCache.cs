using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exile;
using Shared.Interfaces;
using SharpDX;

namespace Shared.Cache
{
    public class StaticCache<T> : IStaticCache<T>
    {
        private readonly int _lifeTimeForCache;
        private readonly int _limit;
        private readonly string name;
        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        private bool IsEmpty = true;
        public StaticCache(int lifeTimeForCache = 120, int limit = 30, string name = null) {
            _lifeTimeForCache = lifeTimeForCache;
            _limit = limit;
            this.name = name ?? typeof(T).Name;
            cache = new MemoryCache(this.name);
            _policy = new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromSeconds(lifeTimeForCache), RemovedCallback = arguments => { _DeletedCache++; }
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

        public int Count => FuncCache - _DeletedCache;
        public int DeletedCache => _DeletedCache;

        private int SavedCache;
        private int FuncCache;
        private int _DeletedCache;
        public int ReadCache => SavedCache;
        public int ReadMemory => FuncCache;
        public string CoeffString => $"{Coeff:0.000}% Read from memory";
        public float Coeff => ReadMemory / (float) (ReadCache + ReadMemory) * 100;

        private MemoryCache cache;
        private CacheItemPolicy _policy;

        public T Read(string addr, Func<T> func) {
            cacheLock.EnterReadLock();
            try
            {
                IsEmpty = false;
                var o = cache[addr];
                if (o != null)
                {
                    SavedCache++;
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
                    SavedCache++;
                    return (T) ob;
                }

                try
                {
                    cacheLock.EnterWriteLock();
                    var tt = func();
                    FuncCache++;
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


        public bool Remove(string key) {
            var remove = cache.Remove(key);
            if (remove != null)
                return true;
            return false;
        }
    }
}