using System.Runtime.InteropServices;
using SharpDX;
using System;
using GameOffsets.Native;
namespace GameOffsets
{
   [StructLayout(LayoutKind.Explicit,Pack = 1)]
   public struct ModsComponentOffsets
   {
      public static readonly int HumanStats = 0x20;
      [FieldOffset(0x30)] public long UniqueName;
      [FieldOffset(0x88)] public bool Identified;
      [FieldOffset(0x8C)] public int ItemRarity;
      [FieldOffset(0x90)] public NativePtrArray implicitMods;
      [FieldOffset(0xA8)] public NativePtrArray explicitMods;
        [FieldOffset(0x170)] public NativePtrArray GetImplicitStats;
      [FieldOffset(0x1A0)] public NativePtrArray GetStats;
      [FieldOffset(0x1B8)] public NativePtrArray GetCraftedStats;
      [FieldOffset(0x1D0)] public NativePtrArray GetFracturedStats;
      [FieldOffset(0x434)] public int ItemLevel;
      [FieldOffset(0x438)] public int RequiredLevel;
      [FieldOffset(0x370)] public byte IsUsable;
      [FieldOffset(0x371)] public byte IsMirrored;
      


   }
}