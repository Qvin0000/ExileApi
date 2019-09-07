using System;
using System.Runtime.InteropServices;

namespace GameOffsets.Native
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct NativePtrArray : IEquatable<NativePtrArray>
  {
    public readonly long First;
    public readonly long Last;
    public readonly long End;

    public long Size => Last - First;
    public override string ToString() =>  $"First: 0x{First}, Last: 0x{Last}, End: 0x{End} Size:{Size}"; 

    public bool Equals(NativePtrArray other)
    {
      if (this.First == other.First && this.Last == other.Last)
        return this.End == other.End;
      return false;
    }

    public override bool Equals(object obj)
    {
      if (obj is NativePtrArray other)
        return this.Equals(other);
      return false;
    }

    public override int GetHashCode()
    {
      return (this.First.GetHashCode() * 397 ^ this.Last.GetHashCode()) * 397 ^ this.End.GetHashCode();
    }
  }
}