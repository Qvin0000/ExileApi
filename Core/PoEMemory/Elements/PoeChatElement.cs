namespace ExileCore.PoEMemory.Elements
{
    public class PoeChatElement : Element
    {
        public long TotalMessageCount => ChildCount;

        public EntityLabel this[int index]
        {
            get
            {
                if (index < TotalMessageCount)
                    return GetChildAtIndex(index).AsObject<EntityLabel>();

                return null;
            }
        }
    }
}
