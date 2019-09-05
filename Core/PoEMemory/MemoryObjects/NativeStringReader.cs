using Shared.Interfaces;
using Shared.Static;

namespace PoEMemory
{
    public class NativeStringReader
    {
        public static string ReadString(long address, IMemory M) {
            var Size = M.Read<uint>(address + 0x10);
            var Capacity = M.Read<uint>(address + 0x18);

            //var size = Size;
            //if (size == 0)
            //    return string.Empty;
            if ( /*8 <= size ||*/ 8 <= Capacity) //Have no idea how to deal with this
            {
                var readAddr = M.Read<long>(address);
                return M.ReadStringU(readAddr);
            }
            else
                return M.ReadStringU(address);
        }
    }
}