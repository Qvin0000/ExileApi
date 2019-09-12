using SharpDX;

namespace ExileCore.PoEMemory.Components
{
    public class Beam : Component
    {
        public Vector3 BeamStart => M.Read<Vector3>(Address + 0x50);//beam start is actually the entity world pos
        public Vector3 BeamEnd => M.Read<Vector3>(Address + 0x5C);
        public int Unknown1 => M.Read<int>(Address + 0x40);//looks like 2 bools
        public int Unknown2 => M.Read<int>(Address + 0x44);
    }
}
