using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory
{
    public class StatsDat : FileInMemory
    {
        public StatsDat(IMemory m, Func<long> address) : base(m, address)
        {
            loadItems();
        }

        public IDictionary<string, StatRecord> records { get; } = new Dictionary<string, StatRecord>(StringComparer.OrdinalIgnoreCase);
        public IDictionary<int, StatRecord> recordsById { get; } = new Dictionary<int, StatRecord>();

        public StatRecord GetStatByAddress(long address)
        {
            return records.Values.ToList().Find(x => x.Address == address);
        }

        private void loadItems()
        {
            var iCounter = 1;

            foreach (var addr in RecordAddresses())
            {
                var r = new StatRecord(M, addr, iCounter++);
                records[r.Key] = r;
                recordsById[r.ID] = r;
            }
        }

        public class StatRecord
        {
            public StatRecord(IMemory m, long addr, int iCounter)
            {
                Address = addr;

                Key = RemoteMemoryObject.Cache.StringCache.Read($"{nameof(StatsDat)}{addr + 0}",
                    () => m.ReadStringU(m.Read<long>(addr + 0), 255));

				Flag0 = m.Read<byte>(addr + 0x8) != 0;
				IsLocal = m.Read<byte>(addr + 0x9) != 0;
				IsWeaponLocal = m.Read<byte>(addr + 0xA) != 0;
                Type = Key.Contains("%") ? StatType.Percents : (StatType) m.Read<int>(addr + 0xB);
				Flag3 = m.Read<byte>(addr + 0xF) != 0;

                UserFriendlyName =
                    RemoteMemoryObject.Cache.StringCache.Read($"{nameof(StatsDat)}{addr + 0x10}",
                        () => m.ReadStringU(m.Read<long>(addr + 0x10), 255));

                ID = iCounter;
            }

            public string Key { get; }
            public long Address { get; }
            public StatType Type { get; }
            public bool Flag0 { get; }
            public bool IsLocal { get; }
            public bool IsWeaponLocal { get; }
            public bool Flag3 { get; }
            public string UserFriendlyName { get; }

            // more fields can be added (see in visualGGPK)
            public int ID { get; }

            public override string ToString()
            {
                return string.IsNullOrWhiteSpace(UserFriendlyName) ? Key : UserFriendlyName;
            }

            public string ValueToString(int val)
            {
                switch (Type)
                {
                    case StatType.Boolean:
                        return val != 0 ? "True" : "False";

                    case StatType.IntValue:
                    case StatType.Value2:
                        return val.ToString("+#;-#");
                    case StatType.Percents:
                    case StatType.Precents5:
                        return val.ToString("+#;-#") + "%";
                }

                return "";
            }
        }
    }
}
