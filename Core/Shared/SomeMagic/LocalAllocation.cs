using System;
using System.Runtime.InteropServices;

namespace ExileCore.Shared.SomeMagic
{
    public class LocalAllocation : IDisposable
    {
        public LocalAllocation(int size)
        {
            Size = size;
            AllocationBase = Marshal.AllocHGlobal(Size);
        }

        public int Size { get; }
        public IntPtr AllocationBase { get; private set; }

        public void Dispose()
        {
            Marshal.FreeHGlobal(AllocationBase);
            AllocationBase = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        ~LocalAllocation()
        {
            Dispose();
        }

        public byte[] Read()
        {
            var bytes = new byte[Size];
            Marshal.Copy(AllocationBase, bytes, 0, Size);
            return bytes;
        }

        public T Read<T>()
        {
            return (T) Marshal.PtrToStructure(AllocationBase, typeof(T));
        }

        public void Write(byte[] bytes)
        {
            Marshal.Copy(bytes, 0, AllocationBase, bytes.Length);
        }

        public void Write<T>(T generic)
        {
            Marshal.StructureToPtr(generic, AllocationBase, false);
        }
    }
}
