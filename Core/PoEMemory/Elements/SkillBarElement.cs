namespace ExileCore.PoEMemory.Elements
{
    public class SkillBarElement : Element
    {
        public long TotalSkills => ChildCount;
        public SkillElement this[int k] => Children[k].AsObject<SkillElement>();
    }
}
