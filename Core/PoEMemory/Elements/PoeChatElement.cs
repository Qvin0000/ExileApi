using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.Elements
{
    public class PoeChatElement : Element
    {
        public Element ChatBox => GetChildFromIndices(1, 2, 1);      //0x2C8, 0x3F8
        public Element ChatTextField => GetChildAtIndex(2);
        public string ChatTextMessage => ChatTextField.Text;

        public long TotalMessageCount => ChatBox.ChildCount;

        public IList<string> ChatMessages => ChatBox.Children.Select(x => x.Text).ToList();
        public IList<string> VisibleChatMessages => ChatBox.Children.Where(x => x.IsVisible).Select(x => x.Text).ToList();
        
    }
}
