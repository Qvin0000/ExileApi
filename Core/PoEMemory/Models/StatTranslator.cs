using System.Collections.Generic;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.Models
{
    public class StatTranslator
    {
        private readonly Dictionary<string, AddStat> mods;

        public StatTranslator()
        {
            mods = new Dictionary<string, AddStat>
            {
                {"Dexterity", Single(ItemStatEnum.Dexterity)},
                {"Strength", Single(ItemStatEnum.Strength)},
                {"Intelligence", Single(ItemStatEnum.Intelligence)},
                {"IncreasedMana", Single(ItemStatEnum.AddedMana)},
                {"IncreasedLife", Single(ItemStatEnum.AddedHP)},
                {"IncreasedEnergyShield", Single(ItemStatEnum.AddedES)},
                {"IncreasedEnergyShieldPercent", Single(ItemStatEnum.AddedESPercent)},
                {"ColdResist", Single(ItemStatEnum.ColdResistance)},
                {"FireResist", Single(ItemStatEnum.FireResistance)},
                {"LightningResist", Single(ItemStatEnum.LightningResistance)},
                {"ChaosResist", Single(ItemStatEnum.ChaosResistance)},
                {
                    "AllResistances",
                    MultipleSame(ItemStatEnum.ColdResistance, ItemStatEnum.FireResistance, ItemStatEnum.LightningResistance)
                },
                {"CriticalStrikeChance", Single(ItemStatEnum.CritChance)},
                {"LocalCriticalMultiplier", Single(ItemStatEnum.CritMultiplier)},
                {"MovementVelocity", Single(ItemStatEnum.MovementSpeed)},
                {"ItemFoundRarityIncrease", Single(ItemStatEnum.Rarity)},
                {"ItemFoundQuantityIncrease", Single(ItemStatEnum.Quantity)},
                {"ManaLeech", Single(ItemStatEnum.ManaLeech)},
                {"LifeLeech", Single(ItemStatEnum.LifeLeech)},
                {"AddedLightningDamage", Average(ItemStatEnum.AddedLightningDamage)},
                {"AddedColdDamage", Average(ItemStatEnum.AddedColdDamage)},
                {"AddedFireDamage", Average(ItemStatEnum.AddedFireDamage)},
                {"AddedPhysicalDamage", Average(ItemStatEnum.AddedPhysicalDamage)},
                {"WeaponElementalDamage", Single(ItemStatEnum.WeaponElementalDamagePercent)},
                {"FireDamagePercent", Single(ItemStatEnum.FireDamagePercent)},
                {"ColdDamagePercent", Single(ItemStatEnum.ColdDamagePercent)},
                {"LightningDamagePercent", Single(ItemStatEnum.LightningDamagePercent)},
                {"SpellDamage", Single(ItemStatEnum.SpellDamage)},
                {"SpellDamageAndMana", Dual(ItemStatEnum.SpellDamage, ItemStatEnum.AddedMana)},
                {"SpellCriticalStrikeChance", Single(ItemStatEnum.SpellCriticalChance)},
                {"IncreasedCastSpeed", Single(ItemStatEnum.CastSpeed)},
                {"ProjectileSpeed", Single(ItemStatEnum.ProjectileSpeed)},
                {"LocalIncreaseSocketedMinionGemLevel", Single(ItemStatEnum.MinionSkillLevel)},
                {"LocalIncreaseSocketedFireGemLevel", Single(ItemStatEnum.FireSkillLevel)},
                {"LocalIncreaseSocketedColdGemLevel", Single(ItemStatEnum.ColdSkillLevel)},
                {"LocalIncreaseSocketedLightningGemLevel", Single(ItemStatEnum.LightningSkillLevel)},
                {"LocalAddedPhysicalDamage", Average(ItemStatEnum.LocalPhysicalDamage)},
                {"LocalIncreasedPhysicalDamagePercent", Single(ItemStatEnum.LocalPhysicalDamagePercent)},
                {"LocalAddedColdDamage", Average(ItemStatEnum.LocalAddedColdDamage)},
                {"LocalAddedFireDamage", Average(ItemStatEnum.LocalAddedFireDamage)},
                {"LocalAddedLightningDamage", Average(ItemStatEnum.LocalAddedLightningDamage)},
                {"LocalCriticalStrikeChance", Single(ItemStatEnum.LocalCritChance)},
                {"LocalIncreasedAttackSpeed", Single(ItemStatEnum.LocalAttackSpeed)},
                {"LocalIncreasedEnergyShield", Single(ItemStatEnum.LocalES)},
                {"LocalIncreasedEvasionRating", Single(ItemStatEnum.LocalEV)},
                {"LocalIncreasedPhysicalDamageReductionRating", Single(ItemStatEnum.LocalArmor)},
                {"LocalIncreasedEvasionRatingPercent", Single(ItemStatEnum.LocalEVPercent)},
                {"LocalIncreasedEnergyShieldPercent", Single(ItemStatEnum.LocalESPercent)},
                {"LocalIncreasedPhysicalDamageReductionRatingPercent", Single(ItemStatEnum.LocalArmorPercent)},
                {"LocalIncreasedArmourAndEvasion", MultipleSame(ItemStatEnum.LocalArmorPercent, ItemStatEnum.LocalEVPercent)},
                {"LocalIncreasedArmourAndEnergyShield", MultipleSame(ItemStatEnum.LocalArmorPercent, ItemStatEnum.LocalESPercent)},
                {"LocalIncreasedEvasionAndEnergyShield", MultipleSame(ItemStatEnum.LocalEVPercent, ItemStatEnum.LocalESPercent)}
            };
        }

        public void Translate(ItemStats stats, ItemMod m)
        {
            if (!mods.ContainsKey(m.Name)) return;
            mods[m.Name](stats, m);
        }

        private AddStat Single(ItemStatEnum stat)
        {
            return delegate(ItemStats x, ItemMod m) { x.AddToMod(stat, m.Value1); };
        }

        private AddStat Average(ItemStatEnum stat)
        {
            return delegate(ItemStats x, ItemMod m) { x.AddToMod(stat, (m.Value1 + m.Value2) / 2f); };
        }

        private AddStat Dual(ItemStatEnum s1, ItemStatEnum s2)
        {
            return delegate(ItemStats x, ItemMod m)
            {
                x.AddToMod(s1, m.Value1);
                x.AddToMod(s2, m.Value2);
            };
        }

        private AddStat MultipleSame(params ItemStatEnum[] stats)
        {
            return delegate(ItemStats x, ItemMod m)
            {
                foreach (var stat in stats)
                {
                    x.AddToMod(stat, m.Value1);
                }
            };
        }

        private delegate void AddStat(ItemStats stats, ItemMod m);
    }
}
