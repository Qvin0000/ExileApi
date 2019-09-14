using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.Elements
{
    public class InventoryElement : Element
    {
        private InventoryList _allInventories;
        private InventoryList AllInventories => _allInventories = _allInventories ?? GetObjectAt<InventoryList>(0x340);
        public Inventory this[InventoryIndex k] => AllInventories[k];
    }
}
