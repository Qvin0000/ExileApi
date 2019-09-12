namespace ExileCore.PoEMemory.Elements
{
    public class IncursionWindow : Element
    {
        public Element AcceptElement
        {
            get
            {
                try
                {
                    var button = GetChildFromIndices(3, 13, 2);

                    if (button.GetChildAtIndex(0).Text == "enter incursion")
                        return button;
                }
                catch
                {
                }

                return null;
            }
        }

        public string Reward1 => GetChildFromIndices(3, 13, 3).Text;
        public string Reward2 => GetChildFromIndices(3, 13, 4).Text;
    }
}
