namespace ExileCore.PoEMemory.MemoryObjects
{
    public class SellWindow : Element
    {
        public Element SellDialog => GetChildAtIndex(3);
        public Element YourOffer => SellDialog?.GetChildAtIndex(0);
        public Element OtherOffer => SellDialog?.GetChildAtIndex(1);
        public string NameSeller => SellDialog?.GetChildAtIndex(2).Text;
        public Element AcceptButton => SellDialog?.GetChildAtIndex(5);
        public Element CancelButton => SellDialog?.GetChildAtIndex(6);
    }
}
