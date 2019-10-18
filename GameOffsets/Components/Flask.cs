namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;
    using GameOffsets.Native;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Flask
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;

        //// FlaskBuffsStruct
        [FieldOffset(0x0010)] public NativePtrArray ExtraBuffsDueToModsPtr; // 0x00 for white flask
        [FieldOffset(0x0028)] public long FlaskInternalPtr; // Ptr to Flask.dat info
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FlaskInternalStruct
    {
        [FieldOffset(0x0018)] public NativePtrArray FlaskInfoPtr;
        [FieldOffset(0x0030)] public int FlaskType; // FlaskGroup
        //// 0x0034 no idea what those 4 bytes are.
        [FieldOffset(0x0038)] public FlaskBuffsStruct FlaskBuffDueToType; // 0x00 in case of Life/Mana/Hybrid flask
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FlaskInfoStruct
    {
        [FieldOffset(0x0004)] public int LifePerUse;
        [FieldOffset(0x000C)] public int ManaPerUse;
        [FieldOffset(0x0014)] public int RecoveryTime;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FlaskBuffsStruct
    {
        //// If this pointer is 0x00 then there is No Buff information to look for.
        //// I think this is a pointer to some File but we don't care about that.
        [FieldOffset(0x0000)] public long IsValidPtr;
        //// BuffDefination.DAT file
        [FieldOffset(0x0008)] public long BuffIdStringPtr;
        [FieldOffset(0x0010)] public NativePtrArray BuffValuePtr;
    }
}