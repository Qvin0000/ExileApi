namespace PoEMemory.Components
{
    public class Weapon : Component
    {
        public int DamageMin => Address != 0 ? M.Read<int>(Address + 0x28, 0x14) : 0;
        public int DamageMax => Address != 0 ? M.Read<int>(Address + 0x28, 0x18) : 0;
        public int AttackTime => Address != 0 ? M.Read<int>(Address + 0x28, 0x1C) : 1;
        public int CritChance => Address != 0 ? M.Read<int>(Address + 0x28, 0x20) : 0;
    }
}