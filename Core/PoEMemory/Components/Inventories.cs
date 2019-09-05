namespace PoEMemory.Components
{
    public class Inventories : Component
    {
        public InventoryVisual LeftHand => ReadVisual(0);
        public InventoryVisual RightHand => ReadVisual(1);
        public InventoryVisual Chest => ReadVisual(2);
        public InventoryVisual Helm => ReadVisual(3);
        public InventoryVisual Gloves => ReadVisual(4);
        public InventoryVisual Boots => ReadVisual(5);
        public InventoryVisual Unknown => ReadVisual(6);
        public InventoryVisual LeftRing => ReadVisual(7);
        public InventoryVisual RightRing => ReadVisual(8);
        public InventoryVisual Belt => ReadVisual(9);

        internal InventoryVisual ReadVisual(int index) {
            index++; //Mean (Address + 0x40 + index * 0x40)
            return ReadObject<InventoryVisual>(Address + index * 0x40);
        }
    }
}