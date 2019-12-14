namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ComponentHeader
    {
        [FieldOffset(0x0000)] public long StaticPtr;
        [FieldOffset(0x0008)] public long EntityPtr;
    }
}