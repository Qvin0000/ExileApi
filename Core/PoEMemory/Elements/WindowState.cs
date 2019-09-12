namespace ExileCore.PoEMemory.Elements
{
    public class WindowState : Element
    {
        public new bool IsVisibleLocal => M.Read<int>(Address + 0x860) == 1;
    }
}
