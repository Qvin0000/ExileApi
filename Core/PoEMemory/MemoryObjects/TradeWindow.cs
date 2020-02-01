using System.Collections.Generic;
using ExileCore.PoEMemory.Elements.InventoryElements;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class TradeWindow : Element
    {
        public Element TradeDialog => GetChildAtIndex(3)?.GetChildAtIndex(1)?.GetChildAtIndex(0)?.GetChildAtIndex(0);
        public Element YourOffer => TradeDialog?.GetChildAtIndex(0);
        public Element OtherOffer => TradeDialog?.GetChildAtIndex(1);
        public string NameSeller => TradeDialog?.GetChildAtIndex(2)?.Text;
        public Element AcceptButton => TradeDialog?.GetChildAtIndex(5);
        public Element CancelButton => TradeDialog?.GetChildAtIndex(6);


        public IList<NormalInventoryItem> YourOfferItems
        {
            get
            {
                var InvRoot = YourOffer;

                if (InvRoot == null || InvRoot.Address == 0x00)
                    return null;

                var list = new List<NormalInventoryItem>();

                foreach (var item in InvRoot.Children)
                {
                    if (item.ChildCount == 0) continue;
                    var normalItem = item.AsObject<NormalInventoryItem>();

                    list.Add(normalItem);
                }

                return list;
            }
        }

        public IList<NormalInventoryItem> OtherOfferItems
        {
            get
            {
                var InvRoot = OtherOffer;

                if (InvRoot == null || InvRoot.Address == 0x00)
                    return null;

                var list = new List<NormalInventoryItem>();

                foreach (var item in InvRoot.Children)
                {
                    if (item.ChildCount == 0) continue;
                    var normalItem = item.AsObject<NormalInventoryItem>();

                    list.Add(normalItem);
                }

                return list;
            }
        }
    }
}