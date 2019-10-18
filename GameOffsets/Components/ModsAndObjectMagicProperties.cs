namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;
    using GameOffsets.Native;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Mods
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0028)] public long  ModsComponentInternalPtr; //ModsComponentInternalStruct
        [FieldOffset(0x0030)] public ModsAndObjectMagicPropertiesCommonStruct ModsInformation;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ModsComponentInternalStruct
    {
        [FieldOffset(0x0038)] public byte InventoryType; //InventoryType.DAT
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ObjectMagicProperties
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0018)] public long  Unknown0Ptr; // Same for same Metadata String
        [FieldOffset(0x0020)] public ModsAndObjectMagicPropertiesCommonStruct ModsInformation;
    }

    //// If looking at this Struct from Mods component, Add 0x30 to each offset.
    //// If looking at this Struct from ObjectMagicProperties component, Add 0x20 to each offset.
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ModsAndObjectMagicPropertiesCommonStruct
    {
        //// An Array containing Pointers to Words.dat row
        //// First Ptr of the array is garbage so start from 0x08
        //// Increment by 0x10 as every 2nd Ptr of the array is garbage too.
        public static readonly int WordsPtrStart = 0x08;
        public static readonly int WordsPtrIncrement = 0x10;
        [FieldOffset(0x0000)] public NativePtrArray WordsPtr; //WordsStruct
        [FieldOffset(0x0058)] public bool Identified;
        [FieldOffset(0x005C)] public int ItemRarity;

        // Following three NativePtrArray are one after the other.
        //    (maybe put them in a local struct)
        // They have 0x00 address/value if those mods are missing.
        [FieldOffset(0x0060)] public NativePtrArray ImplicitModsPtr; //Array of ItemModsStruct
        [FieldOffset(0x0078)] public NativePtrArray ExplicitModsPtr; //Array of ItemModsStruct
        [FieldOffset(0x0090)] public NativePtrArray EnchantmentModsPtr; //Array of ItemModsStruct

        // . 0x00 = key, 0x04 = value
        //   0x08 = key, 0x0C = value
        //   and so on.
        //...Remember: Subtract 1 from the key to get real key present in the Stats.dat file
        [FieldOffset(0x00F0)] public NativePtrArray StatsPtr; //Array of Stats.dat stuff

        // Not sure if these are the monsters spawned by this mod or
        // Monster on which this mod can spawn.
        [FieldOffset(0x0120)] public NativePtrArray MonsterVarietiesPtr;

         //Mods as list of string
         //An Array of NativeUnicodeText
        [FieldOffset(0x0140)] public NativePtrArray ImplicitModsStringPtr;
        [FieldOffset(0x0158)] public NativePtrArray EnchantmentModsStringPtr;
        [FieldOffset(0x0170)] public NativePtrArray ExplicitModsStringPtr;
        [FieldOffset(0x0188)] public NativePtrArray CraftedModsStringPtr;
        [FieldOffset(0x01A0)] public NativePtrArray FracturedModsStringPtr;

        // There are other other Mods related information
        // e.g. Mod Tier, Mod string help text etc etc
        // but we don't really need it.
        // Not tested, all 4 of them.
        //[FieldOffset(0x0434)] public int ItemLevel;
        //[FieldOffset(0x0438)] public int RequiredLevel;
        //[FieldOffset(0x0370)] public byte IsUsable;
        //[FieldOffset(0x0371)] public byte IsMirrored;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct WordsStruct
    {
        [FieldOffset(0x0000)] public int WorldListKey;
        [FieldOffset(0x0004)] public long Text1;
        [FieldOffset(0x000C)] public SpawnWeightStruct SpawnWeightPtr;
        [FieldOffset(0x002C)] public int Unknown0; // Name taken from PyPoe
        [FieldOffset(0x0030)] public long Text2; // Name taken from PyPoe
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct SpawnWeightStruct
    {
        [FieldOffset(0x0000)] public long TotalTags; // yes these are 8byte long
        [FieldOffset(0x0008)] public long TagsPtr;
        [FieldOffset(0x0010)] public long TotalValues; // yes these are 8bytes long
        [FieldOffset(0x0018)] public long ValuePtr;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ItemModsStruct
    {
        [FieldOffset(0x0000)] public NativePtrArray ModValues;
        //[FieldOffset(0x0018)] public long UselessPtr;
        [FieldOffset(0x0020)] public long NamePtr; //ModInfoStruct
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ModInfoStruct //Mods.dat row
    {
        [FieldOffset(0x0000)] public long IdStringPtr;
        [FieldOffset(0x0008)] public int Unknown0;
        [FieldOffset(0x0014)] public long ModTypePtr;
        [FieldOffset(0x001C)] public int MinLevelRequired;
        [FieldOffset(0x0028)] public long StatsDatPtr1;
        [FieldOffset(0x0038)] public long StatsDatPtr2;
        [FieldOffset(0x0048)] public long StatsDatPtr3;
        [FieldOffset(0x0058)] public long StatsDatPtr4;
        // Details: http://pathofexile.gamepedia.com/Modifiers#Mod_Domain
        [FieldOffset(0x0060)] public int Domain;
        [FieldOffset(0x0064)] public long NamePtr;
        // Details: http://pathofexile.gamepedia.com/Modifiers#Mod_Generation_Type
        [FieldOffset(0x006C)] public int GenerationType;
        [FieldOffset(0x0070)] public long CorrectionGroupPtr;
        [FieldOffset(0x0078)] public MinMaxStruct StatRange1;
        [FieldOffset(0x0080)] public MinMaxStruct StatRange2;
        [FieldOffset(0x0088)] public MinMaxStruct StatRange3;
        [FieldOffset(0x0090)] public MinMaxStruct StatRange4;
        [FieldOffset(0x0098)] public SpawnWeightStruct SpawnWeightPtr;
        [FieldOffset(0x00B8)] public long BuffDefinationKey;
        [FieldOffset(0x00C0)] public long BuffDefinationValue;
    }
}