using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Shared.Cache;
using PoEMemory;
using Shared.Helpers;
using Shared.Interfaces;

namespace Shared.Interfaces
{
    public class Cache
    {
        public Cache() => CreateCache();


        public void CreateCache() {
            StaticCacheElements = new StaticCache<RemoteMemoryObject>(300, 60, "Elements");
            StaticCacheComponents = new StaticCache<RemoteMemoryObject>(90, 29, "Components");
            StaticEntityCache = new StaticCache<RemoteMemoryObject>(60, 30, "Entity");
            StaticEntityListCache = new StaticCache<RemoteMemoryObject>(60, 30, "Entities parse");
            StaticServerEntityCache = new StaticCache<RemoteMemoryObject>(90, 30, "Server entities parse");
            StringCache = new StaticCache<string>(300);
        }

        public IStaticCache<RemoteMemoryObject> StaticCacheElements { get; private set; }
        public IStaticCache<RemoteMemoryObject> StaticCacheComponents { get; private set; }
        public IStaticCache<RemoteMemoryObject> StaticEntityCache { get; private set; }
        public IStaticCache<RemoteMemoryObject> StaticEntityListCache { get; private set; }
        public IStaticCache<RemoteMemoryObject> StaticServerEntityCache { get; private set; }
        public IStaticCache<string> StringCache { get; private set; }


        public void TryClearCache() {
            StaticCacheElements.UpdateCache();
            StaticCacheComponents.UpdateCache();
            StaticEntityCache.UpdateCache();
            StaticEntityListCache.UpdateCache();
            StaticServerEntityCache.UpdateCache();
            StringCache.UpdateCache();
        }
    }
}