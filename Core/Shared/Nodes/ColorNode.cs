using System.Drawing;
using SharpDX;
using Color = SharpDX.Color;

namespace ExileCore.Shared.Nodes
{
    public sealed class ColorNode
    {
        private Color _value;

        public ColorNode()
        {
        }

        public ColorNode(uint color)
        {
            Value = Color.FromAbgr(color);
        }

        public ColorNode(Color color)
        {
            Value = color;
        }

        public string Hex { get; private set; }

        public Color Value
        {
            get => _value;
            set
            {
                Hex = ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B));
                _value = value;
            }
        }

        public static implicit operator Color(ColorNode node)
        {
            return node.Value;
        }

        public static implicit operator ColorNode(uint value)
        {
            return new ColorNode(value);
        }

        public static implicit operator ColorNode(Color value)
        {
            return new ColorNode(value);
        }

        public static implicit operator ColorNode(ColorBGRA value)
        {
            return new ColorNode(value);
        }
    }
}
