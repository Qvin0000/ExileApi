namespace ExileCore.PoEMemory.Elements
{
    public class ItemOnGroundTooltip : Element
    {
        public Element ItemFrame => GetChildAtIndex(0) == null ? null : GetChildAtIndex(0).GetChildAtIndex(0);
        public Element Tooltip => GetChildAtIndex(0) == null ? null : GetChildAtIndex(0).GetChildAtIndex(1);
        public Element TooltipUI => GetChildAtIndex(0) == null ? null : GetChildAtIndex(0).GetChildAtIndex(0);
    }
}
