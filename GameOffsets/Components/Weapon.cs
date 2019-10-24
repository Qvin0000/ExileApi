namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Weapon
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0018)] public long LocalStatsComponentPtr;
        [FieldOffset(0x0028)] public long WeaponInternalPtr; //WeaponInternalStruct
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct WeaponInternalStruct
    {
        [FieldOffset(0x0008)] public long Unknown0Ptr;
        // Following Variables found in Metadata/Item/Weapons/AbstarctWeapon.ot
        // These are base weapon stats, without applying the modifier
        [FieldOffset(0x0010)] public int Unknown1;
        [FieldOffset(0x0014)] public int minimum_damage;
        [FieldOffset(0x0018)] public int maximum_damage;
        [FieldOffset(0x001C)] public int weapon_speed;
        [FieldOffset(0x0020)] public int critical_chance;
        [FieldOffset(0x0024)] public int minimum_attack_distance;
        [FieldOffset(0x0028)] public int maximum_attack_distance;
        // Couldn't find this.
        //[FieldOffset(0x0000)] public int accuracy_rating; //20
    }
}