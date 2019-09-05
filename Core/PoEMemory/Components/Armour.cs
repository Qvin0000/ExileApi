namespace PoEMemory.Components
{
    public class Armour : Component
    {
        public int EvasionScore => Address != 0 ? M.Read<int>(Address + 0x10, 0x10) : 0;

        public int ArmourScore => Address != 0 ? M.Read<int>(Address + 0x10, 0x14) : 0;

        public int EnergyShieldScore => Address != 0 ? M.Read<int>(Address + 0x10, 0x18) : 0;
    }
}