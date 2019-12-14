namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Chest
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;

        //// This Ptr can be zero for a chest with the same Metadata path.
        //// So I am not sure what this Ptr is for. I have a feeling this
        //// is reminant of an old GGG code. Anyway, nothing useful inside it.
        //[FieldOffset(0x0010)] public long BrokenPtr;

        //// Theory: Whenever there is a StaticPtr in a component,
        //// it means some other structure is creating/using/pointing to
        //// this structure from here/this offset.
        //[FieldOffset(0x0018)] public long StaticPtr2;

        //// This is an array of pointers. If chest is a normal chest then it only contains
        //// the tartgetable component ptr in this Array @ offset 0.
        //// Whereas, if chest is a Strongbox chest then it contains
        //// the targetable component ptr @ offset 0 and some other ptr @ offset 8.
        //// We can use size of this NativePtrArray to figure out if it's a chest or a strongbox.
        //[FieldOffset(0x0020)] public NativePtrArray ArrayOfPtr;

        //// Chest.dat data in here. it's also present @ 0x88.
        [FieldOffset(0x0058)] public long ChestInternalPtr;
        [FieldOffset(0x0078)] public bool IsOpened;
        [FieldOffset(0x0079)] public bool IsLocked;
        [FieldOffset(0x007C)] public byte Quality;

        //// 0x90, 0x98 ptrs are only available for strongboxes.
        //// However, they disappear when we open the strongbox.

        //// Strongbox/Simple Chest Rarity (look at Rarity.dat).
        //// Always 0 for Simple chests.
        [FieldOffset(0x00B0)] public byte Rarity;

        //// ChestEffects.dat data in here. It's useless.
        //[FieldOffset(0x00B8)] public long ChestEffectsPtr;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ChestInternalStruct
    {
        //// No plugin uses this
        //[FieldOffset(0x20)] public bool DestroyingAfterOpen;
        //[FieldOffset(0x21)] public bool IsLarge;
        //[FieldOffset(0x22)] public bool Stompable;
        //[FieldOffset(0x25)] public bool OpenOnDamage;
        //// Strongboxes.dat data in here.
        //// Can be used to accurately determined if it's a strongbox or not.
        [FieldOffset(0x0050)] public long StrongboxesPtr;
    }
}