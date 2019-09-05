namespace PoEMemory.Components
{
    public class Shrine : Component
    {
        public bool IsAvailable => Address != 0 && M.Read<byte>(Address + 0x24) == 0;
    }
}