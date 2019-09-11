using System;

namespace ExileCore.Shared.Enums
{
    [Flags]
    public enum InventoryTabPermissions : uint
    {
        Add = 2,
        None = 0,
        Remove = 4,
        View = 1
    }

    public enum InventoryTabType : uint
    {
        Currency = 3,
        Divination = 6,
        Essence = 8,
        Fragment = 9,
        Map = 5,
        Normal = 0,
        Premium = 1,
        Quad = 7,
        Todo2 = 2,
        Todo4 = 4
    }

    [Flags]
    public enum InventoryTabFlags : byte
    {
        Hidden = 0x80,
        MapSeries = 0x40,
        Premium = 4,
        Public = 0x20,
        RemoveOnly = 1,
        Unknown1 = 0x10,
        Unknown2 = 2,
        Unknown3 = 8
    }
}
