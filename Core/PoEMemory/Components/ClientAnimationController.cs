namespace ExileCore.PoEMemory.Components
{
    public class ClientAnimationController : Component
    {
        public int AnimKey => M.Read<int>(Address + 0x9c);
    }
}
