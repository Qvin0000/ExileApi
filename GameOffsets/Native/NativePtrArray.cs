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

        public override string ToString()
        {
            return $"First: 0x{First}, Last: 0x{Last}, End: 0x{End} Size:{Size}";
        }

        public bool Equals(NativePtrArray other)
        {
            if (First == other.First && Last == other.Last)
                return End == other.End;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is NativePtrArray other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return (((First.GetHashCode() * 397) ^ Last.GetHashCode()) * 397) ^ End.GetHashCode();
        }
    }
}
