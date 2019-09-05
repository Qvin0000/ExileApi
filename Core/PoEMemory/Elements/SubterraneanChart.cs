using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exile.PoEMemory.Elements;

namespace PoEMemory.Elements
{
    public class SubterraneanChart : Element
    {
        private DelveElement _grid;

        public DelveElement GridElement =>
            Address != 0 ? _grid ?? (_grid = GetObject<DelveElement>(M.Read<long>(Address + 0x1C0, 0x690))) : null;
    }
}