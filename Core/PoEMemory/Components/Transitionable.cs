namespace PoEMemory.Components
{
    public class Transitionable : Component
    {
        public byte Flag1 => M.Read<byte>(Address + 0x120);
        public byte Flag2 => M.Read<byte>(Address + 0x124);
    }
}