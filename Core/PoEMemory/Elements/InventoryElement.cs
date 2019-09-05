using GameOffsets;
using PoEMemory.Components;
using Shared.Enums;

namespace PoEMemory.Elements
{
    public class InventoryElement : Element
    {
        InventoryList _allInventories;

        private InventoryList AllInventories => _allInventories ??= GetObjectAt<InventoryList>(0x340);

        public Inventory this[InventoryIndex k] => AllInventories[k];
    }
}