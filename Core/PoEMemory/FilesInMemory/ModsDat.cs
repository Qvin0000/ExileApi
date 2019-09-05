using System;
using System.Collections.Generic;
using Shared;
using Shared.Helpers;
using Shared.Interfaces;
using GameOffsets;
using Shared;
using Shared.Enums;
using Shared.Static;

namespace PoEMemory.FilesInMemory
{
    public class ModsDat : FileInMemory
    {
        public IDictionary<string, ModRecord> records { get; } = new Dictionary<string, ModRecord>(StringComparer.OrdinalIgnoreCase);

        public IDictionary<long, ModRecord> DictionaryRecords { get; } = new Dictionary<long, ModRecord>();

        public IDictionary<Tuple<string, ModType>, List<ModRecord>> recordsByTier { get; } =
            new Dictionary<Tuple<string, ModType>, List<ModRecord>>();

        public ModsDat(IMemory m, Func<long> address, StatsDat sDat, TagsDat tagsDat) : base(m, address) => loadItems(sDat, tagsDat);

        public ModRecord GetModByAddress(long address) {
            DictionaryRecords.TryGetValue(address, out var result);
            return result;
        }

        private void loadItems(StatsDat sDat, TagsDat tagsDat) {
            foreach (var addr in RecordAddresses())
            {
                var r = new ModRecord(M, sDat, tagsDat, addr);
                if (records.ContainsKey(r.Key))
                    continue;
                DictionaryRecords.Add(addr, r);
                records.Add(r.Key, r);
                var addToItemIiers = r.Domain != ModDomain.Monster;
                if (!addToItemIiers) continue;
                var byTierKey = Tuple.Create(r.Group, r.AffixType);
                if (!recordsByTier.TryGetValue(byTierKey, out var groupMembers))
                {
                    groupMembers = new List<ModRecord>();
                    recordsByTier[byTierKey] = groupMembers;
                }

                groupMembers.Add(r);
            }

            foreach (var list in recordsByTier.Values) list.Sort(ModRecord.ByLevelComparer);
        }


        public class ModRecord
        {
            public long Address { get; }
            public const int NumberOfStats = 4;
            public static IComparer<ModRecord> ByLevelComparer = new LevelComparer();
            public string Key { get; }
            public ModType AffixType { get; }
            public ModDomain Domain { get; }
            public string Group { get; }
            public int MinLevel { get; }
            public StatsDat.StatRecord[] StatNames { get; } // Game refers to Stats.dat line
            public IntRange[] StatRange { get; }
            public IDictionary<string, int> TagChances { get; }
            public TagsDat.TagRecord[] Tags { get; } // Game refers to Tags.dat line
            public long Unknown8 { get; }            //Unknown pointer
            public string UserFriendlyName { get; }
            public bool IsEssence { get; }
            public string Tier { get; }

            public string TypeName { get; }
            // more fields can be added (see in visualGGPK)

            public ModRecord(IMemory m, StatsDat sDat, TagsDat tagsDat, long addr) {
                Address = addr;
                var ModsRecord = m.Read<ModsRecordOffsets>(addr);

                Key = RemoteMemoryObject.Cache.StringCache.Read($"{nameof(ModsDat)}{ModsRecord.Key.buf}",
                                                                () => ModsRecord
                                                                      .Key.ToString(
                                                                          m)); // ModsRecord.Key.ToString(m);// m.ReadStringU(m.Read<long>(addr + 0));
                Unknown8 = ModsRecord.Unknown8;                                // m.Read<long>(addr + 0x8);
                MinLevel = ModsRecord.MinLevel;                                // m.Read<int>(addr + 0x1C);
                // TypeName = m.ReadStringU(m.Read<long>(ModsRecord.TypeName /*m.Read<long>(addr + 0x14*/, 0),255);
                var read = m.Read<long>(ModsRecord.TypeName);
                TypeName = RemoteMemoryObject.Cache.StringCache.Read($"{nameof(ModsDat)}{read}", () => m.ReadStringU(read, 255));

                var s1 = ModsRecord.StatNames1 == 0 ? 0 : m.Read<long>(ModsRecord.StatNames1);
                var s2 = ModsRecord.StatNames2 == 0 ? 0 : m.Read<long>(ModsRecord.StatNames2);
                var s3 = ModsRecord.StatNames3 == 0 ? 0 : m.Read<long>(ModsRecord.StatNames3);
                var s4 = ModsRecord.StatName4 == 0 ? 0 : m.Read<long>(ModsRecord.StatName4);
                StatNames = new[]
                {
                    ModsRecord.StatNames1 == 0
                        ? null
                        : sDat.records[RemoteMemoryObject.Cache.StringCache.Read($"{nameof(ModsDat)}{s1}", () => m.ReadStringU(s1))],
                    ModsRecord.StatNames2 == 0
                        ? null
                        : sDat.records[RemoteMemoryObject.Cache.StringCache.Read($"{nameof(ModsDat)}{s2}", () => m.ReadStringU(s2))],
                    ModsRecord.StatNames3 == 0
                        ? null
                        : sDat.records[RemoteMemoryObject.Cache.StringCache.Read($"{nameof(ModsDat)}{s3}", () => m.ReadStringU(s3))],
                    ModsRecord.StatName4 == 0
                        ? null
                        : sDat.records[RemoteMemoryObject.Cache.StringCache.Read($"{nameof(ModsDat)}{s4}", () => m.ReadStringU(s4))]
                };

                Domain = (ModDomain) ModsRecord.Domain; //m.Read<int>(addr + 0x60);

                UserFriendlyName = RemoteMemoryObject.Cache.StringCache.Read($"{nameof(ModsDat)}{ModsRecord.UserFriendlyName}",
                                                                             () => m.ReadStringU(
                                                                                 ModsRecord
                                                                                     .UserFriendlyName)); //m.ReadStringU(ModsRecord.UserFriendlyName/*m.Read<long>(addr + 0x64)*/);

                AffixType = (ModType) ModsRecord.AffixType; //m.Read<int>(addr + 0x6C);
                Group = RemoteMemoryObject.Cache.StringCache.Read($"{nameof(ModsDat)}{ModsRecord.Group}",
                                                                  () => m.ReadStringU(ModsRecord.Group /*m.Read<long>(addr + 0x70)*/));

                StatRange = new[]
                {
                    /*new IntRange(m.Read<int>(addr + 0x78), m.Read<int>(addr + 0x7C)),
                    new IntRange(m.Read<int>(addr + 0x80), m.Read<int>(addr + 0x84)),
                    new IntRange(m.Read<int>(addr + 0x88), m.Read<int>(addr + 0x8C)),
                    new IntRange(m.Read<int>(addr + 0x90), m.Read<int>(addr + 0x94))*/
                    new IntRange(ModsRecord.StatRange1, ModsRecord.StatRange2), new IntRange(ModsRecord.StatRange3, ModsRecord.StatRange4),
                    new IntRange(ModsRecord.StatRange5, ModsRecord.StatRange6), new IntRange(ModsRecord.StatRange7, ModsRecord.StatRange8)
                };

                //Tags = new TagsDat.TagRecord[m.Read<long>(addr + 0x98)];
                Tags = new TagsDat.TagRecord[ModsRecord.Tags];
                var ta = ModsRecord.ta; // m.Read<long>(addr + 0xA0);
                for (var i = 0; i < Tags.Length; i++)
                {
                    var ii = ta + 0x8 + 0x10 * i;
                    var l = m.Read<long>(ii, 0);
                    Tags[i] = tagsDat.Records[
                        RemoteMemoryObject.Cache.StringCache.Read($"{nameof(ModsDat)}{l}", () => m.ReadStringU(l, 255))];
                }

                //  TagChances = new Dictionary<string,int>(m.Read<int>(addr + 0xA8));
                TagChances = new Dictionary<string, int>(ModsRecord.TagChances);
                var tc = ModsRecord.tc; //m.Read<long>(addr + 0xB0);
                for (var i = 0; i < Tags.Length; i++) TagChances[Tags[i].Key] = m.Read<int>(tc + 4 * i);

                IsEssence = ModsRecord.IsEssence == 0x01; // m.Read<byte>(addr + 0x1AC) == 0x01;
                // Tier = m.ReadStringU(m.Read<long>(addr + 0x1C5));
                Tier = RemoteMemoryObject.Cache.StringCache.Read($"{nameof(ModsDat)}{ModsRecord.Tier}",
                                                                 () => m.ReadStringU(ModsRecord.Tier)); // m.ReadStringU(ModsRecord.Tier);
            }


            public override string ToString() => $"Name: {UserFriendlyName}, Key: {Key}, MinLevel: {MinLevel}";

            private class LevelComparer : IComparer<ModRecord>
            {
                public int Compare(ModRecord x, ModRecord y) => -x.MinLevel + y.MinLevel;
            }
        }
    }
}