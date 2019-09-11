using System;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory
{
    public abstract class RemoteMemoryObject
    {
        private long _address;

        public long Address
        {
            get => _address;
            protected set
            {
                if (_address != value)
                {
                    _address = value;
                    OnAddressChange();
                }
            }
        }

        public static Cache Cache => pCache;
        public IMemory M => pM;
        public TheGame TheGame => pTheGame;
        public static TheGame pTheGame { get; protected set; }
        protected static Cache pCache { get; set; }
        protected static IMemory pM { get; set; }

        protected virtual void OnAddressChange()
        {
        }

        // public static ConcurrentDictionary<string,int> CountType = new ConcurrentDictionary<string, int>();
        public T ReadObjectAt<T>(int offset) where T : RemoteMemoryObject, new()
        {
            return ReadObject<T>(Address + offset);
        }

        public T ReadObject<T>(long addressPointer) where T : RemoteMemoryObject, new()
        {
            var pointer = M.Read<long>(addressPointer);

            // CountType.AddOrUpdate(typeof(T).FullName, 1, (s, oldValue) => oldValue + 1);
            /*
            if (typeof(T) == typeof(Element) && pointer != 0)
            {
                
                var key = string.Join("_", typeof(T).Name, pointer.ToString(),GetHashCode());
                var read = (T)Cache.StaticCacheElements.Read(key,
                                                             () => new Element {Address = pointer});
                /*if (read is Element element && !element.IsValid)
                {
                    Cache.StaticCacheElements.Remove(key);
                    read = new T {Address = pointer};
                }#1#
                return read;
            }
            else if (typeof(T) == typeof(Entity) && pointer != 0)
            {
                /*return (T) Cache.StaticEntityCache.Read(String.Join("_", typeof(T).Name, pointer.ToString(), addressPointer.ToString()),
                                                       () => new Entity {Address = pointer});#1#
                unchecked
                {
                    var type = typeof(T);
                    return (T) Cache.StaticEntityCache.Read($"{type.FullName}{pointer}{addressPointer}",() => new Entity {Address = pointer}); 
                }
        
            }
*/

            var t = new T {Address = pointer};
            return t;
        }

        public T GetObjectAt<T>(int offset) where T : RemoteMemoryObject, new()
        {
            return GetObject<T>(Address + offset);
        }

        public T GetObjectAt<T>(long offset) where T : RemoteMemoryObject, new()
        {
            return GetObject<T>(Address + offset);
        }

        public T GetObject<T>(long address) where T : RemoteMemoryObject, new()
        {
            //    CountType.AddOrUpdate(typeof(T).FullName, 1, (s, oldValue) => oldValue + 1);
            /*if (typeof(T) == typeof(Element) && address != 0)
            {
                var key = string.Join("_", typeof(T).Name, address.ToString(),GetHashCode());
                var read = (T)Cache.StaticCacheElements.Read(key,
                                                             () => new Element {Address = address});
                /*if (read is Element element && !element.IsValid)
                {
                    Cache.StaticCacheElements.Remove(key);
                    read = new T {Address = address};
                }#1#
                return read;
            }*/

            if (address >= long.MaxValue || address < 0x100000000) address = 0;
            var t = new T {Address = address};
            return t;
        }

        public T GetObject<T>(IntPtr address) where T : RemoteMemoryObject, new()
        {
            return GetObject<T>(address.ToInt64());
        }

        public T AsObject<T>() where T : RemoteMemoryObject, new()
        {
            //     CountType.AddOrUpdate(typeof(T).FullName, 1, (s, oldValue) => oldValue + 1);
            /*
            if (typeof(T) == typeof(Element) && Address != 0)
            {
                var key = string.Join("_", typeof(T).Name, Address.ToString(),GetHashCode());
                var read = (T)Cache.StaticCacheElements.Read(key,() => new Element {Address = Address});
                /*if (read is Element element && !element.IsValid)
                {
                    Cache.StaticCacheElements.Remove(key);
                    read = new T {Address = Address};
                }#1#
                return read;
            }
*/

            var t = new T {Address = Address};
            return t;
        }

        public override bool Equals(object obj)
        {
            return obj is RemoteMemoryObject remoteMemoryObject && remoteMemoryObject.Address == Address;
        }

        public override int GetHashCode()
        {
            return (int) Address + GetType().Name.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Address:X}";
        }
    }
}
