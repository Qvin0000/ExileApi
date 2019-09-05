using Shared.Enums;
using SharpDX;

namespace Shared.Helpers
{
    public static class SpriteHelper
    {
        public static RectangleF GetUV(MyMapIconsIndex index) => GetUV((int) index, Constants.MyMapIcons);
        public static RectangleF GetUV(MapIconsIndex index) => GetUV((int) index, Constants.MapIconsSize);

        public static RectangleF GetUV(int index, Size2F size) {
            if (index % (int) size.Width == 0)
            {
                return new RectangleF((size.Width - 1) / size.Width, ((int) (index / size.Width) - 1) / size.Height, 1 / size.Width,
                                      1 / size.Height);
            }

            return new RectangleF((index % size.Width - 1) / size.Width, index / (int) size.Width / size.Height, 1 / size.Width,
                                  1 / size.Height);
        }

        public static RectangleF GetUV(Size2 index, Size2F size) =>
            new RectangleF((index.Width - 1) / size.Width, (index.Height - 1) / size.Height, 1 / size.Width, 1 / size.Height);

        public static RectangleF GetUV(int x, int y, float width, float height) =>
            new RectangleF((x - 1) / width, (y - 1) / height, 1 / width, 1 / height);
    }
}