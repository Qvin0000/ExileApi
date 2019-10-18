namespace GameOffsets.Components
{
     using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct AttributeRequirements
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0010)] public long RequirementsInternalPtr;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct RequirementsInternalStruct
    {
        [FieldOffset(0x0010)] public int strength;
        [FieldOffset(0x0014)] public int dexterity;
        [FieldOffset(0x0018)] public int intelligence;
    }
}