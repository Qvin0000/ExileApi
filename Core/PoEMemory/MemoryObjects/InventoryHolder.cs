namespace ExileCore.PoEMemory.MemoryObjects
{
    public class InventoryHolder : RemoteMemoryObject
    {
        internal const int StructSize = 0x20;
        public int Id => M.Read<int>(Address);
        public ServerInventory Inventory => ReadObject<ServerInventory>(Address + 0x8);

        public override string ToString()
        {
            return
                $"InventoryType: {Inventory.InventType}, InventorySlot: {Inventory.InventSlot}, IsRequested: {Inventory.IsRequested}, ItemsCount: {Inventory.Items.Count} CountItems: {Inventory.CountItems}";
        }
    }
}
