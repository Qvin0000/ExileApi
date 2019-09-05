using System.Text;
using PoEMemory.FilesInMemory;

namespace PoEMemory
{
    public class BestiaryRecipeComponent : RemoteMemoryObject
    {
        public int Id { get; internal set; }

        private string recipeId = null;
        public string RecipeId => recipeId != null ? recipeId : recipeId = M.ReadStringU(M.Read<long>(Address));

        private int minLevel = -1;
        public int MinLevel => minLevel != -1 ? minLevel : minLevel = M.Read<int>(Address + 0x8);

        private BestiaryFamily bestiaryFamily;

        public BestiaryFamily BestiaryFamily =>
            bestiaryFamily != null
                ? bestiaryFamily
                : bestiaryFamily = (BestiaryFamily) TheGame.Files.BestiaryFamilies.GetByAddress(M.Read<long>(Address + 0x14));

        private BestiaryGroup bestiaryGroup;

        public BestiaryGroup BestiaryGroup =>
            bestiaryGroup != null
                ? bestiaryGroup
                : bestiaryGroup = (BestiaryGroup) TheGame.Files.BestiaryGroups.GetByAddress(M.Read<long>(Address + 0x24));

        private BestiaryGenus bestiaryGenus;

        public BestiaryGenus BestiaryGenus =>
            bestiaryGenus != null
                ? bestiaryGenus
                : bestiaryGenus = (BestiaryGenus) TheGame.Files.BestiaryGenuses.GetByAddress(M.Read<long>(Address + 0x58));

        //Can be null, not all have mods
        private ModsDat.ModRecord mod;

        public ModsDat.ModRecord Mod =>
            mod != null ? mod : mod = (ModsDat.ModRecord) TheGame.Files.Mods.GetModByAddress(M.Read<long>(Address + 0x34));

        private BestiaryCapturableMonster bestiaryCapturableMonster;

        public BestiaryCapturableMonster BestiaryCapturableMonster =>
            bestiaryCapturableMonster ?? (bestiaryCapturableMonster =
                (BestiaryCapturableMonster) TheGame.Files.BestiaryCapturableMonsters.GetByAddress(M.Read<long>(Address + 0x44)));

        public override string ToString() {
            var debugStr = new StringBuilder();
            debugStr.Append($"Id: {Id}, ");


            if (MinLevel > 0)
                debugStr.Append($"MinLevel: {MinLevel}, ");

            if (Mod != null)
                debugStr.Append($"Mod: {Mod.ToString()}, ");

            if (BestiaryCapturableMonster != null)
                debugStr.Append($"MonsterName: {BestiaryCapturableMonster.MonsterName}, ");

            if (BestiaryFamily != null)
                debugStr.Append($"BestiaryFamily: {BestiaryFamily.Name}, ");

            if (BestiaryGroup != null)
                debugStr.Append($"BestiaryGroup: {BestiaryGroup.Name}, ");

            if (BestiaryGenus != null)
                debugStr.Append($"BestiaryGenus: {BestiaryGenus.Name}, ");

            return debugStr.ToString();
        }
    }
}