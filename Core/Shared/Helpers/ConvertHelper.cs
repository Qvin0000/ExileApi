using System;
using System.Drawing;
using System.Globalization;
using Shared.Nodes;
using Shared.Nodes;
using SharpDX;
using Color = SharpDX.Color;
using Vector3 = System.Numerics.Vector3;

namespace Shared.Helpers
{
    public static class ConvertHelper
    {
        public static string ToShorten(double value, string format = "0") {
            var abs = Math.Abs(value);
            if (abs >= 1000000) return string.Concat((value / 1000000).ToString("F2"), "M");

            if (abs >= 1000) return string.Concat((value / 1000).ToString("F1"), "K");

            return value.ToString(format);
        }

        public static Color ToBGRAColor(this string value) =>
            uint.TryParse(value, NumberStyles.HexNumber, null, out var bgra) ? Color.FromBgra(bgra) : Color.Black;

        public static Color? ConfigColorValueExtractor(this string[] line, int index) =>
            IsNotNull(line, index) ? (Color?) line[index].ToBGRAColor() : null;

        public static string ConfigValueExtractor(this string[] line, int index) => IsNotNull(line, index) ? line[index] : null;

        private static bool IsNotNull(string[] line, int index) => line.Length > index && !string.IsNullOrEmpty(line[index]);

        public static Vector3 ColorNodeToVector3(this ColorNode color) {
            var vector3 = color.Value.ToVector3();
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        public static System.Numerics.Vector2 TranslateToNum(this Vector2 vector, float dx = 0, float dy = 0) =>
            new System.Numerics.Vector2(vector.X + dx, vector.Y + dy);

        public static System.Numerics.Vector4 TranslateToNum(this Vector4 vector, float dx = 0, float dy = 0, float dz = 0, float dw = 0) =>
            new System.Numerics.Vector4(vector.X + dx, vector.Y + dy, vector.Z + dz, vector.W + dw);

        public static Vector3 TranslateToNum(this Vector3 vector, float dx = 0, float dy = 0, float dz = 0) =>
            new Vector3(vector.X + dx, vector.Y + dy, vector.Z + dz);

        public static string ToHex(this Color value) =>
            ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B));

        public static Color ColorFromHsv(double hue, double saturation, double value) {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            var v = Convert.ToByte(value);
            var p = Convert.ToByte(value * (1 - saturation));
            var q = Convert.ToByte(value * (1 - f * saturation));
            var t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0:
                    return new ColorBGRA(v, t, p, 255);

                case 1:
                    return new ColorBGRA(q, v, p, 255);

                case 2:
                    return new ColorBGRA(p, v, t, 255);

                case 3:
                    return new ColorBGRA(p, q, v, 255);

                case 4:
                    return new ColorBGRA(t, p, v, 255);

                default:
                    return new ColorBGRA(v, p, q, 255);
            }
        }
    }
}