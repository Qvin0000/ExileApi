namespace PoEMemory.Components
{
    public class Quality : Component
    {
        public int ItemQuality => Address != 0 ? M.Read<int>(Address + 0x18) : 0;
    }
}