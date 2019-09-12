namespace ExileCore.PoEMemory.Components
{
    public class CurrencyInfo : Component
    {
        public int MaxStackSize => Address != 0 ? M.Read<int>(Address + 0x28) : 0;
    }
}
