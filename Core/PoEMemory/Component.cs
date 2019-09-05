using System.Text;
using Exile.PoEMemory.MemoryObjects;
using Shared.Interfaces;

namespace PoEMemory
{
    public class Component : RemoteMemoryObject
    {
        public long OwnerAddress => M.Read<long>(Address + 0x8);
        public Entity Owner => ReadObject<Entity>(Address + 8);

        public string DumpObject() {
            var type = GetType();
            var propertyInfos = type.GetProperties();
            var strs = new StringBuilder();
            foreach (var propertyInfo in propertyInfos)
            {
                var value = propertyInfo.GetValue(this, null);

                if (value is RemoteMemoryObject)
                {
                    strs.AppendLine($"{propertyInfo.Name} => {value}");
                    strs.AppendLine($"ToString => {value.GetType().GetMethod("ToString").Invoke(this, null)}");
                }
                else
                    strs.AppendLine($"{propertyInfo.Name} => {value}");
            }

            return strs.ToString();
        }
    }
}