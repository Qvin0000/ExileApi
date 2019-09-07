using System;
using System.Runtime.InteropServices;
using GameOffsets.Native;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ObjectMagicPropertiesOffsets
    {

        [FieldOffset(0x7C)] public int Rarity;
        [FieldOffset(0x98)] public NativePtrArray Mods;

    }
    
}