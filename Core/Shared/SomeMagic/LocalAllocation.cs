using System;
using System.Runtime.InteropServices;

namespace Shared.SomeMagic
{
    public class LocalAllocation : IDisposable
    {
        public int Size { get; private set; }
        public IntPtr AllocationBase { get; private set; }

        public LocalAllocation(int size) {
            Size = size;
            AllocationBase = Marshal.AllocHGlobal(Size);
        }

        ~LocalAllocation() => Dispose();

        public void Dispose() {
            Marshal.FreeHGlobal(AllocationBase);
            AllocationBase = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        public byte[] Read() {
            var bytes = new byte[Size];
            Marshal.Copy(AllocationBase, bytes, 0, Size);
            return bytes;
        }

        public T Read<T>() => (T) Marshal.PtrToStructure(AllocationBase, typeof(T));

        public void Write(byte[] bytes) => Marshal.Copy(bytes, 0, AllocationBase, bytes.Length);

        public void Write<T>(T generic) => Marshal.StructureToPtr(generic, AllocationBase, false);
    }
}