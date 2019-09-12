using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.PoEMemory.Models;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components
{
    public class Mods : Component
    {
        private readonly CachedValue<ModsComponentOffsets> _cachedValue;

        public Mods()
        {
            _cachedValue = new FrameCache<ModsComponentOffsets>(() => M.Read<ModsComponentOffsets>(Address));
        }

        public ModsComponentOffsets ModsStruct => _cachedValue.Value;

        //  public string UniqueName => Address != 0 ? M.ReadStringU(M.Read<long>(Address + 0x30, 0x8, 0x4)) + M.ReadStringU(M.Read<long>(Address + 0x30, 0x18, 4)) : string.Empty;
        //   public string UniqueName => Address != 0 ? M.ReadStringU(M.Read<long>(Address + 0x30, 0x8,0x30)) + M.ReadStringU(M.Read<long>(Address + 0x30, 0x18,0x30)) : string.Empty;
        public string UniqueName =>
            Address != 0
                ? Cache.StringCache.Read($"{nameof(Mods)}{ModsStruct.UniqueName + 0x8}",
                    () => M.ReadStringU(M.Read<long>(ModsStruct.UniqueName + 0x8, 0x30)) +
                          M.ReadStringU(M.Read<long>(ModsStruct.UniqueName + 0x18, 0x30)))
                : string.Empty;

        // public bool Identified => Address != 0 && M.Read<byte>(Address + 0x88) == 1;
        public bool Identified => Address != 0 && ModsStruct.Identified;
        public ItemRarity ItemRarity => Address != 0 ? (ItemRarity) ModsStruct.ItemRarity : ItemRarity.Normal;

        //Usefull for cache items
        public long Hash => ModsStruct.implicitMods.GetHashCode() ^ ModsStruct.explicitMods.GetHashCode() ^ ModsStruct.GetHashCode();

        public List<ItemMod> ItemMods
        {
            get
            {
                var implicitMods = GetMods(ModsStruct.implicitMods.First, ModsStruct.implicitMods.Last);
                var explicitMods = GetMods(ModsStruct.explicitMods.First, ModsStruct.explicitMods.Last);
                return implicitMods.Concat(explicitMods).ToList();
            }
        }

        public int ItemLevel => Address != 0 ? ModsStruct.ItemLevel /*M.Read<int>(Address + 0x42c) */ : 1;
        public int RequiredLevel => Address != 0 ? ModsStruct.RequiredLevel /*M.Read<int>(Address + 0x430)*/ : 1;
        public bool IsUsable => Address != 0 && M.Read<byte>(Address + 0x370) == 1;
        public bool IsMirrored => Address != 0 && M.Read<byte>(Address + 0x371) == 1;
        public int CountFractured => M.Read<byte>(Address + 0x89);
        public bool Synthesised => M.Read<byte>(Address + 0x437) == 1;
        public bool HaveFractured => Address != 0 && CountFractured > 0;
        public ItemStats ItemStats => new ItemStats(Owner);
        public List<string> HumanStats => GetStats(ModsStruct.GetStats);
        public List<string> HumanCraftedStats => GetStats(ModsStruct.GetCraftedStats);
        public List<string> HumanImpStats => GetStats(ModsStruct.GetImplicitStats);
        public List<string> FracturedStats => GetStats(ModsStruct.GetFracturedStats);

        private List<string> GetStats(NativePtrArray array)
        {
            var readPointersArray = M.ReadPointersArray(array.First, array.Last, ModsComponentOffsets.HumanStats);
            var result = new List<string>();

            foreach (var pointer in readPointersArray)
            {
                result.Add(Cache.StringCache.Read($"{nameof(Mods)}{pointer}", () => M.ReadStringU(pointer)));
            }

            return result;
        }

        private List<ItemMod> GetMods(long startOffset, long endOffset)
        {
            var list = new List<ItemMod>();

            if (Address == 0)
                return list;

            var begin = startOffset;
            var end = endOffset;
            var count = (end - begin) / 0x28;

            if (count > 12)
                return list;

            //System.Windows.Forms.MessageBox.Show(begin.ToString("x"));

            for (var i = begin; i < end; i += 0x28)
            {
                list.Add(GetObject<ItemMod>(i));
            }

            return list;
        }
    }
}
