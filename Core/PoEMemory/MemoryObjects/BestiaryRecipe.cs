using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class BestiaryRecipe : RemoteMemoryObject
    {
        private List<BestiaryRecipeComponent> components;
        private string description;
        private string hint;
        private string notes;
        private string recipeId;
        private BestiaryRecipeComponent specialMonster;
        public int Id { get; internal set; }
        public string RecipeId => recipeId != null ? recipeId : recipeId = M.ReadStringU(M.Read<long>(Address));
        public string Description => description != null ? description : description = M.ReadStringU(M.Read<long>(Address + 0x8));
        public string Notes => notes != null ? notes : notes = M.ReadStringU(M.Read<long>(Address + 0x20));
        public string HintText => hint != null ? hint : hint = M.ReadStringU(M.Read<long>(Address + 0x28));
        public bool RequireSpecialMonster => Components.Count == 4;

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

        public override string ToString()
        {
            return HintText + ": " + Description;
        }
    }
}
