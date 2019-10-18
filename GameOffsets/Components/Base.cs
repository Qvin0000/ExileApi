namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;
    using SharpDX;
    using GameOffsets.Native;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Base
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0010)] public long BaseInternalPtr;
        [FieldOffset(0x00D8)] public bool IsCorrupted;
        [FieldOffset(0x00D9)] public bool IsShaper;
        [FieldOffset(0x00DA)] public bool IsElder;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct BaseInternalStruct
    {
        [FieldOffset(0x0010)] public Vector2 Size;
        [FieldOffset(0x0018)] public NativeUnicodeText BaseNamePtr;
        [FieldOffset(0x0038)] public int LevelRequirement;
    }
}