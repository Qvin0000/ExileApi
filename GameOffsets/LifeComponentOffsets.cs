using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    public struct LifeComponentOffsets
    {
        [FieldOffset(0x8)] public long Owner;
        [FieldOffset(0x154)] public int MaxHP;
        [FieldOffset(0x15C)] public int CurHP;
        [FieldOffset(0x90)] public float Regen;
        [FieldOffset(0x158)] public int ReservedFlatHP;
        [FieldOffset(0x160)] public int ReservedPercentHP;
        [FieldOffset(0xBC)] public int MaxMana;
        [FieldOffset(0xC4)] public int CurMana;
        [FieldOffset(0xB8)] public float ManaRegen;
        [FieldOffset(0xC0)] public int ReservedFlatMana;
        [FieldOffset(0xC8)] public int ReservedPercentMana;
        [FieldOffset(0xF4)] public int MaxES;
        [FieldOffset(0xFC)] public int CurES;
        [FieldOffset(0x80)] public NativePtrArray Buffs;
    }
}