namespace ExileCore.PoEMemory.Components
{
    public class AttributeRequirements : Component
    {
        public int strength => Address != 0 ? M.Read<int>(Address + 0x10, 0x10) : 0;
        public int dexterity => Address != 0 ? M.Read<int>(Address + 0x10, 0x14) : 0;
        public int intelligence => Address != 0 ? M.Read<int>(Address + 0x10, 0x18) : 0;
    }
}
