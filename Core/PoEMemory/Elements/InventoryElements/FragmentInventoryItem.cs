using SharpDX;

namespace ExileCore.PoEMemory.Elements.InventoryElements
{
    public class FragmentInventoryItem : NormalInventoryItem
    {
        // Inventory Position in Currency Stash is always invalid.
        // Also, as items are fixed, so Inventory Position doesn't matter.
        public override int InventPosX => 0;
        public override int InventPosY => 0;

        public override RectangleF GetClientRect()
        {
            return Parent.GetClientRect();
        }
    }
}
