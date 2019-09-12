namespace ExileCore.PoEMemory.Components
{
    public class NPC : Component
    {
        public bool HasIconOverhead => M.Read<long>(Address + 0x48) != 0;
        public bool IsIgnoreHidden => M.Read<byte>(Address + 0x20) == 1;
        public bool IsMinMapLabelVisible => M.Read<byte>(Address + 0x21) == 1;
    }
}
