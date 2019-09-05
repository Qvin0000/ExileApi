using System.Collections.Generic;
using System.Linq;
using Exile.PoEMemory.MemoryObjects;


namespace PoEMemory.Elements
{
    public class HPbarElement : Element
    {
        public Entity MonsterEntity => ReadObject<Entity>(Address + 0x96C);
        public new List<HPbarElement> Children => GetChildren<HPbarElement>().Cast<HPbarElement>().ToList();
    }
}