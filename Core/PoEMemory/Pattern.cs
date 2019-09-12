using System;
using System.Globalization;
using System.Linq;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory
{
    public class Pattern : IPattern
    {
        public Pattern(byte[] pattern, string mask, string name, int startOffset = 0)
        {
            Bytes = pattern;
            Mask = mask;
            Name = name;
            StartOffset = startOffset;
        }

        public Pattern(string pattern, string mask, string name, int startOffset = 0)
        {
            var arr = pattern.Split(new[] {"\\x"}, StringSplitOptions.RemoveEmptyEntries);
            Bytes = arr.Select(y => byte.Parse(y, NumberStyles.HexNumber)).ToArray();
            Mask = mask;
            Name = name;
            StartOffset = startOffset;
        }

        public string Name { get; }
        public byte[] Bytes { get; }
        public string Mask { get; }
        public int StartOffset { get; }
    }
}
