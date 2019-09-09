using System;
using System.Collections.Generic;
using Exile;
using GameOffsets;
using JM.LinqFaster;
using ProcessMemoryUtilities.Memory;
using Shared.Helpers;
using Shared.Interfaces;

namespace PoEMemory.Components
{
    public class Life : Component
    {
        private static long BuffStartOffset = Extensions.GetOffset<LifeComponentOffsets>(nameof(LifeComponentOffsets.Buffs));
        private static long BuffLastOffset = Extensions.GetOffset<LifeComponentOffsets>(nameof(LifeComponentOffsets.Buffs)) + 0x8;
        private readonly CachedValue<List<Buff>> _cachedValueBuffs;
        private readonly CachedValue<LifeComponentOffsets> _life;

        public Life() {
            _life = new FrameCache<LifeComponentOffsets>(() => Address == 0 ? default : M.Read<LifeComponentOffsets>(Address));
            _cachedValueBuffs = new FrameCache<List<Buff>>(ParseBuffs);
        }

        public long OwnerAddress => LifeComponentOffsetsStruct.Owner;
        private LifeComponentOffsets LifeComponentOffsetsStruct => _life.Value;

        public int MaxHP => Address != 0 ? LifeComponentOffsetsStruct.MaxHP : 1;
        public int CurHP => Address != 0 ? LifeComponentOffsetsStruct.CurHP : 0;
        public int ReservedFlatHP => LifeComponentOffsetsStruct.ReservedFlatHP;

        public int ReservedPercentHP => LifeComponentOffsetsStruct.ReservedPercentHP;

        public int MaxMana => Address != 0 ? LifeComponentOffsetsStruct.MaxMana : 1;
        public int CurMana => Address != 0 ? LifeComponentOffsetsStruct.CurMana : 1;
        public int ReservedFlatMana => LifeComponentOffsetsStruct.ReservedFlatMana;
        public int ReservedPercentMana => LifeComponentOffsetsStruct.ReservedPercentMana;
        public int MaxES => LifeComponentOffsetsStruct.MaxES;
        public int CurES => LifeComponentOffsetsStruct.CurES;
        public float HPPercentage => CurHP / (float) (MaxHP - ReservedFlatHP - Math.Round(ReservedPercentHP * 0.01 * MaxHP));
        public float MPPercentage => CurMana / (float) (MaxMana - ReservedFlatMana - Math.Round(ReservedPercentMana * 0.01 * MaxMana));
        public float ESPercentage => MaxES == 0 ? 0 : CurES / (float) MaxES;

        //public bool CorpseUsable => M.ReadMem(Address + 0x238, 1)[0] == 1; // Total guess, didn't verify
        private long BuffStart => LifeComponentOffsetsStruct.Buffs.First;
        private long BuffEnd => LifeComponentOffsetsStruct.Buffs.End;
        private long BuffLast => LifeComponentOffsetsStruct.Buffs.Last;
        private long MaxBuffCount => 512; // Randomly bumping to 512 from 32 buffs... no idea what real value is.


        public List<Buff> Buffs => _cachedValueBuffs.Value;

        public List<Buff> ParseBuffs() {
            try
            {
                var length = BuffLast - BuffStart;
                var numBuffs = (int) length / 8;
                if (length <= 0 || numBuffs >= MaxBuffCount || numBuffs <= 0 || BuffEnd <= 0) // * 8 as we buff pointer takes 8 bytes.
                    return new List<Buff>();
                var buffer = new long[numBuffs];
                ProcessMemory.ReadProcessMemoryArray(M.OpenProcessHandle, (IntPtr) BuffStart, buffer, 0, numBuffs);


                var result = new List<Buff>(numBuffs);
                for (var index = 0; index < buffer.Length; index++)
                {
                    var l = buffer[index];
                    var buff = ReadObject<Buff>(l + 0x8);
                    if (buff.Address == 0 || buff.BuffOffsets.Name == 0)
                        continue;

                    if (!string.IsNullOrEmpty(buff.Name)) result.Add(buff);
                }

                return result;
            }
            catch (Exception e)
            {
                DebugWindow.LogError(
                    $"Life Component Buffs problem. {LifeComponentOffsetsStruct.Buffs} Len: {BuffLast - BuffStart} Div: {(BuffLast - BuffStart) / 8} {Environment.NewLine}{e}");
                return null;
            }
        }


        public bool HasBuff(string buff) => Buffs?.AnyF(x => x.Name == buff) ?? false;
    }
}