using System;
using System.Text;

namespace ExileCore.Shared.SomeMagic
{
    public class MemoryLiterate
    {
        private readonly SafeMemoryHandle _safeMemoryHandle;

        public MemoryLiterate(SafeMemoryHandle safeMemoryHandle)
        {
            _safeMemoryHandle = safeMemoryHandle;
        }

        public byte[] Read(IntPtr address, int size)
        {
            var buffer = new byte[size];
            NativeMethods.ReadProcessMemory(_safeMemoryHandle, address, buffer, size);
            return buffer;
        }

        public byte[] Read(Pointer pointer, int size)
        {
            byte[] buffer;

            if (pointer.Offsets.Count == 0)
            {
                buffer = new byte[size];
                NativeMethods.ReadProcessMemory(_safeMemoryHandle, pointer.BaseAddress, buffer, size);
                return buffer;
            }

            var addressSize = MarshalType<IntPtr>.Size;
            buffer = new byte[addressSize];
            NativeMethods.ReadProcessMemory(_safeMemoryHandle, pointer.BaseAddress, buffer, addressSize);
            var address = TypeConverter.BytesToGenericType<IntPtr>(buffer);
            var offsetsCount = pointer.Offsets.Count - 1;

            for (var i = 0; i < offsetsCount; ++i)
            {
                NativeMethods.ReadProcessMemory(_safeMemoryHandle, address + pointer.Offsets[i], buffer, addressSize);
                address = TypeConverter.BytesToGenericType<IntPtr>(buffer);
            }

            buffer = new byte[size];
            NativeMethods.ReadProcessMemory(_safeMemoryHandle, address + pointer.Offsets[offsetsCount], buffer, size);
            return buffer;
        }

        public T Read<T>(IntPtr address) where T : struct
        {
            return TypeConverter.BytesToGenericType<T>(Read(address, MarshalType<T>.Size));
        }

        public T Read<T>(Pointer pointer) where T : struct
        {
            return TypeConverter.BytesToGenericType<T>(Read(pointer, MarshalType<T>.Size));
        }

        public T[] Read<T>(IntPtr address, int count) where T : struct
        {
            var el = new T[count];

            for (var i = 0; i < count; i++)
            {
                el[i] = Read<T>(address + i * MarshalType<T>.Size);
            }

            return el;
        }

        public string Read(IntPtr address, int size, Encoding encoding)
        {
            var buffer = Read(address, size);
            var s = encoding.GetString(buffer);
            var i = s.IndexOf('\0');

            if (i != -1)
                s = s.Remove(i);

            return s;
        }

        public string Read(Pointer pointer, int size, Encoding encoding)
        {
            var buffer = Read(pointer, size);
            var s = encoding.GetString(buffer);
            var i = s.IndexOf('\0');

            if (i != -1)
                s = s.Remove(i);

            return s;
        }

        public bool Write(IntPtr address, byte[] bytes)
        {
            using (new MemoryProtection(_safeMemoryHandle, address, bytes.Length))
            {
                return NativeMethods.WriteProcessMemory(_safeMemoryHandle, address, bytes, bytes.Length) == bytes.Length;
            }
        }

        public bool Write(Pointer pointer, byte[] bytes)
        {
            if (pointer.Offsets.Count == 0)
            {
                using (new MemoryProtection(_safeMemoryHandle, pointer.BaseAddress, bytes.Length))
                {
                    return NativeMethods.WriteProcessMemory(_safeMemoryHandle, pointer.BaseAddress, bytes, bytes.Length) == bytes.Length;
                }
            }

            var addressSize = MarshalType<IntPtr>.Size;
            var address = TypeConverter.BytesToGenericType<IntPtr>(Read(pointer.BaseAddress, addressSize));
            var offsetsCount = pointer.Offsets.Count - 1;

            for (var i = 0; i < offsetsCount; ++i)
            {
                address = TypeConverter.BytesToGenericType<IntPtr>(Read(address + pointer.Offsets[i], addressSize));
            }

            address += pointer.Offsets[offsetsCount];

            using (new MemoryProtection(_safeMemoryHandle, address, bytes.Length))
            {
                return NativeMethods.WriteProcessMemory(_safeMemoryHandle, address, bytes, bytes.Length) == bytes.Length;
            }
        }

        public bool Write<T>(IntPtr address, T value) where T : struct
        {
            return Write(address, TypeConverter.GenericTypeToBytes(value));
        }

        public bool Write<T>(Pointer pointer, T value) where T : struct
        {
            return Write(pointer, TypeConverter.GenericTypeToBytes(value));
        }

        public bool Write(IntPtr address, string value, Encoding encoding)
        {
            if (value[value.Length - 1] != '\0')
                value += '\0';

            var bytes = encoding.GetBytes(value);
            return Write(address, bytes);
        }

        public bool Write(Pointer pointer, string value, Encoding encoding)
        {
            if (value[value.Length - 1] != '\0')
                value += '\0';

            var bytes = encoding.GetBytes(value);
            return Write(pointer, bytes);
        }
    }
}
