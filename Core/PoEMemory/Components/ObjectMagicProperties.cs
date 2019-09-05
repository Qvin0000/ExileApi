using System;
using System.Collections.Generic;
using Exile;
using Shared.Static;
using GameOffsets;
using Shared.Interfaces;
using Shared.Enums;
using SharpDX;

namespace PoEMemory.Components
{
    public class ObjectMagicProperties : Component
    {
        private CachedValue<ObjectMagicPropertiesOffsets> _cachedValue;
        public ObjectMagicPropertiesOffsets ObjectMagicPropertiesOffsets => _cachedValue.Value;

        public ObjectMagicProperties() =>
            _cachedValue = new FrameCache<ObjectMagicPropertiesOffsets>(() => M.Read<ObjectMagicPropertiesOffsets>(Address));

        public MonsterRarity Rarity
        {
            get
            {
                if (Address != 0)
                {
                    // return (MonsterRarity)M.Read<int>(Address + 0x7C);
                    return (MonsterRarity) ObjectMagicPropertiesOffsets.Rarity;
                }

                return MonsterRarity.Error;
            }
        }

        private long _modsHash;
        public long ModsHash => ObjectMagicPropertiesOffsets.Mods.GetHashCode();
        private List<string> modsList = new List<string>();

        public List<string> Mods
        {
            get
            {
                if (Address == 0) return null;

                if (_modsHash == ModsHash) return modsList;

                var begin = ObjectMagicPropertiesOffsets.Mods.First;
                var end = ObjectMagicPropertiesOffsets.Mods.Last;
                if (begin == 0 || end == 0) return new List<string>();
                var j = 0;
                for (var i = begin; i < end; i += 0x28)
                {
                    var read = M.Read<long>(i + 0x20, 0);
                    var mod = Cache.StringCache.Read($"{nameof(ObjectMagicProperties)}{read}", () => M.ReadStringU(read));
                    modsList.Add(mod);
                    j++;
                    if (j > 256)
                    {
                        DebugWindow.LogMsg($"{nameof(ObjectMagicProperties)} read mods error address", 2, Color.OrangeRed);
                        break;
                    }
                }

                _modsHash = ModsHash;
                return modsList;
            }
        }
    }
}