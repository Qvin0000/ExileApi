using SharpDX;

namespace Shared
{
    public class HudTexture
    {
        public string FileName { get; set; }
        public RectangleF UV { get; set; } = new RectangleF(0, 0, 1, 1);
        public float Size { get; set; } = 13;
        public Color Color { get; set; } = Color.White;

        public HudTexture() { }
        public HudTexture(string fileName) => FileName = fileName;
    }
}