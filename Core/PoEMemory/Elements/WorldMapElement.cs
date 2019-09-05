using PoEMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoEMemory.Elements
{
    public class WorldMapElement : Element
    {
        public Element Panel => GetObject<Element>(M.Read<long>(Address + 0xAB8, 0xC10));
    }
}