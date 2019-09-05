namespace PoEMemory.Components
{
    public class Magnetic : Component
    {
        public int Force => M.Read<int>(Address + 0x30);
    }
}