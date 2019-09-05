using System.Collections.Generic;
using System.Linq;

namespace PoEMemory.Elements
{
    public class MapStashTabElementQ : Element
    {
        public Dictionary<string, string> MapsCount => GetMapsCount();
        public Dictionary<string, string> CurrentCell => GetCurrentCell();

        private Dictionary<string, string> GetCurrentCell() {
            var cell = Children[2].Children[0].Children[0].Children;
            var result = new Dictionary<string, string>();
            foreach (var element in cell)
            {
                var name = element?.Tooltip?.Children?[0].Children[0].Children[3].Text;
                if (name == null)
                {
                    var tooltipText = element.Tooltip?.Text;
                    name = tooltipText != null ? tooltipText.Substring(0, tooltipText.IndexOf('\n')) : "Error";
                }


                var count = element.Children[4].Text;
                result.Add(name, count);
            }

            return result;
        }

        private Dictionary<string, string> GetMapsCount() {
            var Rows = Children[0].Children.Concat(Children[1].Children);
            var result = new Dictionary<string, string>();
            foreach (var element in Rows) result.Add(element.Children[0].Text, element.Children[1].Text);

            return result;
        }
    }
}