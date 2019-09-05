using System.Drawing;
using SharpDX;
using Color = SharpDX.Color;

namespace Shared.Nodes
{
    public sealed class ColorNode
    {
        private Color _value;

        private string _hex;
        public string Hex => _hex;

        public ColorNode() { }

        public ColorNode(uint color) => Value = Color.FromAbgr(color);

        public ColorNode(Color color) => Value = color;

        public Color Value
        {
            get => _value;
            set
            {
                _hex = ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B));
                _value = value;
            }
        }

        public static implicit operator Color(ColorNode node) => node.Value;

        public static implicit operator ColorNode(uint value) => new ColorNode(value);

        public static implicit operator ColorNode(Color value) => new ColorNode(value);

        public static implicit operator ColorNode(ColorBGRA value) => new ColorNode(value);
    }
}