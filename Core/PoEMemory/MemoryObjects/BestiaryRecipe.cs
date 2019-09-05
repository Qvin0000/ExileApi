using System.Collections.Generic;
using System.Linq;


namespace PoEMemory
{
    public class BestiaryRecipe : RemoteMemoryObject
    {
        public int Id { get; internal set; }

        private string recipeId = null;
        public string RecipeId => recipeId != null ? recipeId : recipeId = M.ReadStringU(M.Read<long>(Address));

        private string description = null;
        public string Description => description != null ? description : description = M.ReadStringU(M.Read<long>(Address + 0x8));

        private string notes = null;
        public string Notes => notes != null ? notes : notes = M.ReadStringU(M.Read<long>(Address + 0x20));

        private string hint = null;
        public string HintText => hint != null ? hint : hint = M.ReadStringU(M.Read<long>(Address + 0x28));

        public bool RequireSpecialMonster => Components.Count == 4;

        private BestiaryRecipeComponent specialMonster;

        public BestiaryRecipeComponent SpecialMonster
        {
            get
            {
                if (!RequireSpecialMonster) return null;
                if (specialMonster == null)
                    specialMonster = Components.FirstOrDefault();

                return specialMonster;
            }
        }

        private List<BestiaryRecipeComponent> components;

        public IList<BestiaryRecipeComponent> Components
        {
            get
            {
                if (components == null)
                {
                    var count = M.Read<int>(Address + 0x10);
                    var pointers = M.ReadSecondPointerArray_Count(M.Read<long>(Address + 0x18), count);
                    components = pointers.Select(x => TheGame.Files.BestiaryRecipeComponents.GetByAddress(x)).ToList();
                }

                return components;
            }
        }

        public override string ToString() => HintText + ": " + Description;
    }
}