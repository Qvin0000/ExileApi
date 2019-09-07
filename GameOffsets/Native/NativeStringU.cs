using System;
using System.Runtime.InteropServices;

namespace GameOffsets.Native
{
  [StructLayout(LayoutKind.Explicit, Pack = 1)]
  public struct NativeStringU
  {
    [FieldOffset(0x0)]
    public long buf;
    [FieldOffset(0x8)]
    public long buf2;
    [FieldOffset(0x10)]
    public uint Size;
    [FieldOffset(0x18)]
    public uint Capacity;
  }
}
