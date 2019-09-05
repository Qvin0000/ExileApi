using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Exile;
using Shared.Static;
using SharpDX;

namespace Shared.Interfaces
{
    public class StaticStringCache
    {
        private readonly int _lifeTimeForCache;

        private Dictionary<IntPtr, string> cache = new Dictionary<IntPtr, string>();
        public Dictionary<IntPtr, string> Debug => cache;
        public int Count => cache.Count;
        private DateTime lastClear;

        public StaticStringCache(int LifeTimeForCache = 300) => _lifeTimeForCache = LifeTimeForCache;

        public int ClearByTime() {
            var counter = 0;
            if ((DateTime.UtcNow - lastClear).TotalSeconds < 60) return counter;
            foreach (var ptr in _lastAccess)
                if ((DateTime.UtcNow - ptr.Value).TotalSeconds > _lifeTimeForCache)
                {
                    if (cache.Remove(ptr.Key))
                    {
                        counter++;
                        _lastAccess.TryRemove(ptr.Key, out _);
                    }
                }

            if (_lastAccess.Count > 30000)
            {
                _lastAccess.Clear();
                cache.Clear();
                DebugWindow.LogMsg($"Clear CACHE because so big (>30k)", 7, Color.GreenYellow);
            }

            lastClear = DateTime.UtcNow;
            DebugWindow.LogMsg($"StaticStringCache Cleared by time: {counter} [{lastClear}]", 7, Color.Yellow);

            return counter;
        }

        private object locker = new object();

        public string Read(IntPtr addr, Func<string> func) {
            if (cache.TryGetValue(addr, out var result))
            {
                _lastAccess[addr] = DateTime.UtcNow;
                return result;
            }

            result = func();
            lock (locker)
            {
                cache[addr] = result;
                /*if (cache.Count > 1500)
                {
                    ClearByTime();
                }*/
            }

            _lastAccess[addr] = DateTime.UtcNow;
            return result;
        }

        private readonly ConcurrentDictionary<IntPtr, DateTime> _lastAccess = new ConcurrentDictionary<IntPtr, DateTime>();
    }
}