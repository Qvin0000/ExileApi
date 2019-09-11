using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SharpDX;

namespace ExileCore.Shared.Cache
{
    public class StaticStringCache
    {
        private readonly ConcurrentDictionary<IntPtr, DateTime> _lastAccess = new ConcurrentDictionary<IntPtr, DateTime>();
        private readonly int _lifeTimeForCache;
        private DateTime lastClear;
        private readonly object locker = new object();

        public StaticStringCache(int LifeTimeForCache = 300)
        {
            _lifeTimeForCache = LifeTimeForCache;
        }

        public Dictionary<IntPtr, string> Debug { get; } = new Dictionary<IntPtr, string>();
        public int Count => Debug.Count;

        public int ClearByTime()
        {
            var counter = 0;
            if ((DateTime.UtcNow - lastClear).TotalSeconds < 60) return counter;

            foreach (var ptr in _lastAccess)
            {
                if ((DateTime.UtcNow - ptr.Value).TotalSeconds > _lifeTimeForCache)
                {
                    if (Debug.Remove(ptr.Key))
                    {
                        counter++;
                        _lastAccess.TryRemove(ptr.Key, out _);
                    }
                }
            }

            if (_lastAccess.Count > 30000)
            {
                _lastAccess.Clear();
                Debug.Clear();
                DebugWindow.LogMsg("Clear CACHE because so big (>30k)", 7, Color.GreenYellow);
            }

            lastClear = DateTime.UtcNow;
            DebugWindow.LogMsg($"StaticStringCache Cleared by time: {counter} [{lastClear}]", 7, Color.Yellow);

            return counter;
        }

        public string Read(IntPtr addr, Func<string> func)
        {
            if (Debug.TryGetValue(addr, out var result))
            {
                _lastAccess[addr] = DateTime.UtcNow;
                return result;
            }

            result = func();

            lock (locker)
            {
                Debug[addr] = result;
                /*if (cache.Count > 1500)
                {
                    ClearByTime();
                }*/
            }

            _lastAccess[addr] = DateTime.UtcNow;
            return result;
        }
    }
}
