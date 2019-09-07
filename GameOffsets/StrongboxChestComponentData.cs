using System.Runtime.InteropServices;

namespace GameOffsets
{
   [StructLayout(LayoutKind.Explicit,Pack = 1)]
   public struct StrongboxChestComponentData
   {
      [FieldOffset(0x20)] public bool DestroyingAfterOpen;
      [FieldOffset(0x21)] public bool IsLarge;
      [FieldOffset(0x22)] public bool Stompable;
      [FieldOffset(0x25)] public bool OpenOnDamage;
   }
}