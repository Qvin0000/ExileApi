using SharpDX;

namespace ExileCore.PoEMemory.Components
{
    public class TriggerableBlockage : Component
    {
        public bool IsClosed => Address != 0 && M.Read<byte>(Address + 0x30) == 1;
        public bool IsOpened => Address != 0 && M.Read<byte>(Address + 0x30) == 0;
        public Point Min => new Point(M.Read<int>(Address + 0x50), M.Read<int>(Address + 0x54));
        public Point Max => new Point(M.Read<int>(Address + 0x58), M.Read<int>(Address + 0x5C));

        public byte[] Data
        {
            get
            {
                var start = M.Read<long>(Address + 0x38);
                var end = M.Read<long>(Address + 0x40);
                return M.ReadBytes(start, (int) (end - start));
            }
        }
    }
}
