using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExileCore.PoEMemory.Models;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory
{
    public class BaseItemTypes : FileInMemory
    {
        private int tries = 0;

        public BaseItemTypes(IMemory m, Func<long> address) : base(m, address)
        {
            LoadItemTypes();
        }

        public Dictionary<string, BaseItemType> Contents { get; } = new Dictionary<string, BaseItemType>();
        public Dictionary<long, BaseItemType> ContentsAddr { get; } = new Dictionary<long, BaseItemType>();

        public BaseItemType GetFromAddress(long address)
        {
            ContentsAddr.TryGetValue(address, out var type);
            return type;
        }

        public BaseItemType Translate(string metadata)
        {
            if (Contents.Count == 0) LoadItemTypes();

            if (metadata == null) return null;

            if (!Contents.TryGetValue(metadata, out var type))
            {
                Console.WriteLine("Key not found in BaseItemTypes: " + metadata);
                return null;
            }

            return type;
        }

        private void LoadItemTypes()
        {
            foreach (var i in RecordAddresses())
            {
                var key = M.ReadStringU(M.Read<long>(i));

                var baseItemType = new BaseItemType
                {
                    Metadata = key,
                    ClassName = M.ReadStringU(M.Read<long>(i + 0x10, 0)),
                    Width = M.Read<int>(i + 0x18),
                    Height = M.Read<int>(i + 0x1C),
                    BaseName = M.ReadStringU(M.Read<long>(i + 0x20)),
                    DropLevel = M.Read<int>(i + 0x30),
                    Tags = new string[M.Read<long>(i + 0xA8)]
                };

                var ta = M.Read<long>(i + 0xB0);

                for (var k = 0; k < baseItemType.Tags.Length; k++)
                {
                    var ii = ta + 0x8 + 0x10 * k;
                    baseItemType.Tags[k] = M.ReadStringU(M.Read<long>(ii, 0), 255);
                }

                var tmpTags = key.Split('/');
                string tmpKey;

                if (tmpTags.Length > 3)
                {
                    baseItemType.MoreTagsFromPath = new string[tmpTags.Length - 3];

                    for (var k = 2; k < tmpTags.Length - 1; k++)
                    {
                        // This Regex and if condition change Item Path Category e.g. TwoHandWeapons
                        // To tag strings type e.g. two_hand_weapon
                        tmpKey = Regex.Replace(tmpTags[k], @"(?<!_)([A-Z])", "_$1").ToLower().Remove(0, 1);

                        if (tmpKey[tmpKey.Length - 1] == 's')
                            tmpKey = tmpKey.Remove(tmpKey.Length - 1);

                        baseItemType.MoreTagsFromPath[k - 2] = tmpKey;
                    }
                }
                else
                {
                    baseItemType.MoreTagsFromPath = new string[1];
                    baseItemType.MoreTagsFromPath[0] = "";
                }

                ContentsAddr.Add(i, baseItemType);

                if (!Contents.ContainsKey(key)) Contents.Add(key, baseItemType);
            }
        }
    }
}
