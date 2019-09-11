namespace ExileCore.PoEMemory.Components
{
    public class Monolith : Component
    {
        //EssenceTypes: 0x28-0x20 is a range, then read double pointer struct (each second pointer)
        public int OpenStage => M.Read<byte>(Address + 0x70);
        public bool IsOpened => OpenStage == 4; //After killing monsters (or on time) this objects disappear
    }
}
