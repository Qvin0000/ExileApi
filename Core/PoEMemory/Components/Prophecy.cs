using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Components
{
    public class Prophecy : Component
    {
        public ProphecyDat DatProphecy => TheGame.Files.Prophecies.GetByAddress(M.Read<long>(Address + 0x20));
    }
}
