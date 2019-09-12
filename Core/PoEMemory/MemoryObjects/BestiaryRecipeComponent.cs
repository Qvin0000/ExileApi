using System.Text;
using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class BestiaryRecipeComponent : RemoteMemoryObject
    {
        private BestiaryCapturableMonster bestiaryCapturableMonster;
        private BestiaryFamily bestiaryFamily;
        private BestiaryGenus bestiaryGenus;
        private BestiaryGroup bestiaryGroup;
        private int minLevel = -1;

        //Can be null, not all have mods
        private ModsDat.ModRecord mod;
        private string recipeId;
        public int Id { get; internal set; }
        public string RecipeId => recipeId != null ? recipeId : recipeId = M.ReadStringU(M.Read<long>(Address));
        public int MinLevel => minLevel != -1 ? minLevel : minLevel = M.Read<int>(Address + 0x8);
        public BestiaryFamily BestiaryFamily =>
            bestiaryFamily != null
                ? bestiaryFamily
                : bestiaryFamily = TheGame.Files.BestiaryFamilies.GetByAddress(M.Read<long>(Address + 0x14));
        public BestiaryGroup BestiaryGroup =>
            bestiaryGroup != null
                ? bestiaryGroup
                : bestiaryGroup = TheGame.Files.BestiaryGroups.GetByAddress(M.Read<long>(Address + 0x24));
        public BestiaryGenus BestiaryGenus =>
            bestiaryGenus != null
                ? bestiaryGenus
                : bestiaryGenus = TheGame.Files.BestiaryGenuses.GetByAddress(M.Read<long>(Address + 0x58));
        public ModsDat.ModRecord Mod =>
            mod != null ? mod : mod = TheGame.Files.Mods.GetModByAddress(M.Read<long>(Address + 0x34));
        public BestiaryCapturableMonster BestiaryCapturableMonster =>
            bestiaryCapturableMonster ?? (bestiaryCapturableMonster =
                TheGame.Files.BestiaryCapturableMonsters.GetByAddress(M.Read<long>(Address + 0x44)));

        public override string ToString()
        {
            var debugStr = new StringBuilder();
            debugStr.Append($"Id: {Id}, ");

            if (MinLevel > 0)
                debugStr.Append($"MinLevel: {MinLevel}, ");

            if (Mod != null)
                debugStr.Append($"Mod: {Mod}, ");

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
