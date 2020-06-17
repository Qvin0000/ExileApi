using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct LifeComponentOffsets
    {
        [FieldOffset(0x8)] public long Owner;
        [FieldOffset(0x180)] public NativePtrArray Buffs;
        [FieldOffset(0x190)] public float Regen;
        [FieldOffset(0x1B8)] public float ManaRegen;
        [FieldOffset(0x1BC)] public int MaxMana;
        [FieldOffset(0x1C0)] public int ReservedFlatMana;
        [FieldOffset(0x1C4)] public int CurMana;
        [FieldOffset(0x1C8)] public int ReservedPercentMana;
        [FieldOffset(0x1F4)] public int MaxES;
        [FieldOffset(0x1FC)] public int CurES;
        [FieldOffset(0x254)] public int MaxHP;
        [FieldOffset(0x258)] public int ReservedFlatHP;
        [FieldOffset(0x25C)] public int CurHP;
        [FieldOffset(0x260)] public int ReservedPercentHP;
    }
}
