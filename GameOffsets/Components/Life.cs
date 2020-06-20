namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;
    using GameOffsets.Native;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Life
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x180)] public NativePtrArray BuffsPtr;
        [FieldOffset(0x198)] public VitalStruct Mana;
        [FieldOffset(0x1D0)] public VitalStruct EnergyShield;
        [FieldOffset(0x230)] public VitalStruct Health;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct BuffStruct
    {
        [FieldOffset(0x0008)] public long BuffInternalPtr;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct BuffInternalStruct
    {
        //// BuffDefination.DAT file
        [FieldOffset(0x0008)] public long Name;
        [FieldOffset(0x0010)] public float MaxTime;
        [FieldOffset(0x0014)] public float CurrTimer;
        [FieldOffset(0x002C)] public byte Charges; // 2 bytes long but 1 is enough
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct VitalStruct
    {
        [FieldOffset(0x0000)] public long PtrToLifeComponent;
        //// This is greater than zero if Vital is regenerating
        //// For value = 0 or less than 0, Vital isn't regenerating
        [FieldOffset(0x20)] public float Regeneration;
        [FieldOffset(0x24)] public int Total;
        //// e.g. ICICLE MINE reserve flat Vital
        [FieldOffset(0x28)] public int ReservedFlat;
        [FieldOffset(0x2C)] public int Current;
        //// e.g. HERALD reserve % Vital.
        //// ReservedFlat does not change this value.
        [FieldOffset(0x30)] public int ReservedPercent;
    }
}
