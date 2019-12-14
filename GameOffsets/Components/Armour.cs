namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Armour
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0010)] public long ArmourInternalPtr;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ArmourInternalStruct
    {
        [FieldOffset(0x0010)] public int Evasion;
        [FieldOffset(0x0014)] public int Armour;
        [FieldOffset(0x0018)] public int EnergyShield;
        [FieldOffset(0x0018)] public int IncreasedMovementSpeed;
    }
}