namespace PoEMemory.Components
{
    public class TimerComponent : Component
    {
        public float TimeLeft => M.Read<float>(Address + 0x18);
    }
}