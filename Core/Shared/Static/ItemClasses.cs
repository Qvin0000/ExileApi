using System.Collections.Generic;

namespace Shared.Static
{
    public class ItemClass
    {
        public string ClassName { get; set; }
        public string ClassCategory { get; set; }

        public ItemClass(string className, string classCategory) {
            ClassName = className;
            ClassCategory = classCategory;
        }
    }

    public class ItemClasses
    {
        public IDictionary<string, ItemClass> contents { get; }

        public ItemClasses() =>
            contents = new Dictionary<string, ItemClass>()
            {
                {"LifeFlask", new ItemClass("Life Flasks", "Flasks")},
                {"ManaFlask", new ItemClass("Mana Flasks", "Flasks")},
                {"HybridFlask", new ItemClass("Hybrid Flasks", "Flasks")},
                {"Currency", new ItemClass("Currency", "Other")},
                {"Amulet", new ItemClass("Amulets", "Jewellery")},
                {"Ring", new ItemClass("Rings", "Jewellery")},
                {"Claw", new ItemClass("Claws", "One Handed Weapon")},
                {"Dagger", new ItemClass("Daggers", "One Handed Weapon")},
                {"Wand", new ItemClass("Wands", "One Handed Weapon")},
                {"One Hand Sword", new ItemClass("One Hand Swords", "One Handed Weapon")},
                {"Thrusting One Hand Sword", new ItemClass("Thrusting One Hand Swords", "One Handed Weapon")},
                {"One Hand Axe", new ItemClass("One Hand Axes", "One Handed Weapon")},
                {"One Hand Mace", new ItemClass("One Hand Maces", "One Handed Weapon")},
                {"Bow", new ItemClass("Bows", "Two Handed Weapon")},
                {"Staff", new ItemClass("Staves", "Two Handed Weapon")},
                {"Two Hand Sword", new ItemClass("Two Hand Swords", "Two Handed Weapon")},
                {"Two Hand Axe", new ItemClass("Two Hand Axes", "Two Handed Weapon")},
                {"Two Hand Mace", new ItemClass("Two Hand Maces", "Two Handed Weapon")},
                {"Active Skill Gem", new ItemClass("Active Skill Gems", "Gems")},
                {"Support Skill Gem", new ItemClass("Support Skill Gems", "Gems")},
                {"Quiver", new ItemClass("Quivers", "Off-hand")},
                {"Belt", new ItemClass("Belts", "Jewellery")},
                {"Gloves", new ItemClass("Gloves", "Armor")},
                {"Boots", new ItemClass("Boots", "Armor")},
                {"Body Armour", new ItemClass("Body Armours", "Armor")},
                {"Helmet", new ItemClass("Helmets", "Armor")},
                {"Shield", new ItemClass("Shields", "Off-hand")},
                {"SmallRelic", new ItemClass("Small Relics", "")},
                {"MediumRelic", new ItemClass("Medium Relics", "")},
                {"LargeRelic", new ItemClass("Large Relics", "")},
                {"StackableCurrency", new ItemClass("Stackable Currency", "")},
                {"QuestItem", new ItemClass("Quest Items", "")},
                {"Sceptre", new ItemClass("Sceptres", "One Handed Weapon")},
                {"UtilityFlask", new ItemClass("Utility Flasks", "Flasks")},
                {"UtilityFlaskCritical", new ItemClass("Critical Utility Flasks", "")},
                {"Map", new ItemClass("Maps", "Other")},
                {"Unarmed", new ItemClass("", "")},
                {"FishingRod", new ItemClass("Fishing Rods", "")},
                {"MapFragment", new ItemClass("Map Fragments", "Other")},
                {"HideoutDoodad", new ItemClass("Hideout Doodads", "")},
                {"Microtransaction", new ItemClass("Microtransactions", "")},
                {"Jewel", new ItemClass("Jewel", "Other")},
                {"DivinationCard", new ItemClass("Divination Card", "Other")},
                {"LabyrinthItem", new ItemClass("Labyrinth Item", "")},
                {"LabyrinthTrinket", new ItemClass("Labyrinth Trinket", "")},
                {"LabyrinthMapItem", new ItemClass("Labyrinth Map Item", "Other")},
                {"MiscMapItem", new ItemClass("Misc Map Items", "")},
                {"Leaguestone", new ItemClass("Leaguestones", "Other")}
            };
    }
}