namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;
    using GameOffsets.Native;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Monster
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0018)] public long MonsterInternalPtr; //MonsterInternalStruct
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MonsterInternalStruct
    {
        [FieldOffset(0x0008)] public long UnknownStructPtr;
        [FieldOffset(0x0018)] public long MonsterVarietiesPtr; //MonsterVarietiesStruct
        [FieldOffset(0x0020)] public int Level;
        //[FieldOffset(0x0040)] public NativePtrArray MonsterVisualEffectsPtr; // MonsterVisualEffectsStruct
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MonsterVarietiesStruct
    {
        [FieldOffset(0x0000)] public long IdStringPtr;
        [FieldOffset(0x0010)] public long MonsterTypePtr;
        // Commented because no one is using it.
        //[FieldOffset(0x0018)] public int Unknown0;
        //[FieldOffset(0x001C)] public int ObjectSize;
        [FieldOffset(0x0020)] public MinMaxStruct AttackDistance; //Min, Max
        //[FieldOffset(0x0028)] public long FileName1Ptr;
        //[FieldOffset(0x0030)] public long FileName2Ptr;
        //[FieldOffset(0x0038)] public long BaseMonsterTypePtr;
        [FieldOffset(0x0040)] public long TotalMods;
        [FieldOffset(0x0048)] public long ModsPtr; //file:Mods.cs, Struct: ModInfoStruct //0x08, not 0x00
        //[FieldOffset(0x0064)] public int ModelSizeMultiplier;
        //[FieldOffset(0x008C)] public int ExperienceMultiplier
        //[FieldOffset(0x00AC)] public int CriticalStrikeChance
        //[FieldOffset(0x00C4)] public long AISFile
        [FieldOffset(0x00F4)] public long MonsterName;
        //[FieldOffset(0x00FC)] public int DamageMultiplier
        //[FieldOffset(0x0100)] public int LifeMultiplier
        //[FieldOffset(0x0104)] public int AttackSpeed
        // and so on....
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct MonsterTypesStruct
    {
        [FieldOffset(0x0000)] public long IdStringPtr;
        [FieldOffset(0x000C)] public bool IsSummoned;
        [FieldOffset(0x000D)] public int Armour;
        [FieldOffset(0x0011)] public int Evasion;
        [FieldOffset(0x0015)] public int EnergyShield;
        [FieldOffset(0x0019)] public int MovementSpeed;
        [FieldOffset(0x001D)] public long TotalTagsKeys;
        [FieldOffset(0x0025)] public long TagsKeys;
        // Feel free to add this Struct if anyone is using it.
        // Look at MonsterResistance.DAT row as reference
        [FieldOffset(0x0035)] public long MonsterResistancesPtr;
    }

    // Don't know which GGPK.DAT file it is.
    // So giving it a random name i.e. MonsterVisualEffects
    //[StructLayout(LayoutKind.Explicit, Pack = 1)]
    //public struct MonsterVisualEffectsStruct
    //{
        //[FieldOffset(0x0018)] public long IdStringPtr;
    //}
}