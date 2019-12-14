namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Stack
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0010)] public long StackInternalPtr; //StackInternalStructure
        [FieldOffset(0x0018)] public int CurrentCount;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct StackInternalStructure
    {
        [FieldOffset(0x0018)] public long CurrencyItemsDatPtr; //CurrencyItems.Dat
        [FieldOffset(0x0020)] public int MaxStackSizeInCurrencyStashTab;
        [FieldOffset(0x0024)] public int Unknown1;
        [FieldOffset(0x0028)] public int MaxStackSize;
    }
}