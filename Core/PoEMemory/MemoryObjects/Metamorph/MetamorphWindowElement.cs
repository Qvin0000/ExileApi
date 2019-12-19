using System.Collections.Generic;
using ExileCore.PoEMemory.FilesInMemory.Metamorph;

namespace ExileCore.PoEMemory.MemoryObjects.Metamorph
{
    public class MetamorphWindowElement : Element
    {
        public Element MetamorphStash => ReadObjectAt<Element>(0x220);
        public IEnumerable<MetamorphBodyPartStashWindowElement> GetBodyPartStashWindowElements => MetamorphStash.GetChildrenAs<MetamorphBodyPartStashWindowElement>();
    }


    public class MetamorphBodyPartStashWindowElement : Element
    {
        public string BodyPartName => GetChildFromIndices(1, 0).Text;

        public IEnumerable<MetamorphBodyPartElement> GetBodyPartStashWindowElements => GetChildAtIndex(0).GetChildrenAs<MetamorphBodyPartElement>();
        public override string ToString()
        {
            return $"{BodyPartName}";
        }
    }

    public class MetamorphBodyPartElement : Element
    {
        public MetamorphMetaSkill MetaSkill => ReadObjectAt<MetamorphMetaSkill>(0x5D0);
    }
}
