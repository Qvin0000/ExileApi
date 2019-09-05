using System;
using Exile.PoEMemory.MemoryObjects;
using Shared.Interfaces;

namespace Exile.Shared.Cache
{
    public class ValidCache<T>:CachedValue<T>
    {
        private readonly Entity _entity;

        public ValidCache(Entity entity,Func<T> func) : base(func) { _entity = entity; }

        protected override bool Update(bool force) {
            return _entity.IsValid || force;
        }


        
    }
    
    
}