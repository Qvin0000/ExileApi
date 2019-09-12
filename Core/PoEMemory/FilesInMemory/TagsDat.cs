using System;
using System.Collections.Generic;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory
{
    public class TagsDat : FileInMemory
    {
        public TagsDat(IMemory m, Func<long> address) : base(m, address)
        {
            loadItems();
        }

        public Dictionary<string, TagRecord> Records { get; } = new Dictionary<string, TagRecord>(StringComparer.OrdinalIgnoreCase);

        private void loadItems()
        {
            foreach (var addr in RecordAddresses())
            {
                var r = new TagRecord(M, addr);

                if (!Records.ContainsKey(r.Key))
                    Records.Add(r.Key, r);
            }
        }

        public class TagRecord
        {
            // more fields can be added (see in visualGGPK)

            public TagRecord(IMemory m, long addr)
            {
                Key = RemoteMemoryObject.Cache.StringCache.Read($"{nameof(TagsDat)}{addr + 0}",
                    () => m.ReadStringU(m.Read<long>(addr + 0), 255));

                Hash = m.Read<int>(addr + 0x8);
            }

            public string Key { get; }
            public int Hash { get; }
        }
    }
}
