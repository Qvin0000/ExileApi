using System.Text;

namespace ExileCore.PoEMemory.Models
{
    public class BaseItemType
    {
        public string Metadata { get; set; }
        public string ClassName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int DropLevel { get; set; }
        public string BaseName { get; set; }
        public string[] Tags { get; set; }
        public string[] MoreTagsFromPath { get; set; }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("Tags: ");

            foreach (var tag in Tags)
            {
                str.Append(tag);
                str.Append(" ");
            }

            str.Append("More Tags: ");

            foreach (var s in MoreTagsFromPath)
            {
                str.Append(s);
                str.Append(" ");
            }

            return str.ToString();
        }
    }
}
