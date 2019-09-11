using System;
using System.Collections.Generic;

namespace ExileCore.Shared.SomeMagic
{
    public class Pointer : IDisposable
    {
        public Pointer(IntPtr baseAddress, params int[] offsets)
        {
            BaseAddress = baseAddress;

            foreach (var i in offsets)
            {
                Offsets.Add(i);
            }
        }

        public IntPtr BaseAddress { get; }
        public List<int> Offsets { get; } = new List<int>();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~Pointer()
        {
            Dispose();
        }
    }
}
