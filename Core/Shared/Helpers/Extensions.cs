using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exile;
using Exile.PoEMemory.MemoryObjects;
using Exile.Shared.Cache;
using JM.LinqFaster;
using Shared.Enums;
using Shared.Interfaces;

namespace Shared.Helpers
{
    public static class Extensions
    {
        private static Dictionary<string, Color> ColorName { get; } = new Dictionary<string, Color>();
        private static Dictionary<Color, string> ColorHex { get; } = new Dictionary<Color, string>();
        private static Color[] Colors;
        private static Dictionary<string, MapIconsIndex> Icons;

        static Extensions() {
            var fieldInfos = typeof(Color).GetFields(BindingFlags.Public | BindingFlags.Static);

            Colors = new Color[fieldInfos.Length];
            ColorName=new Dictionary<string, Color>(fieldInfos.Length);
            ColorHex=new Dictionary<Color, string>(fieldInfos.Length);
            for (var index = 0; index < fieldInfos.Length; index++)
            {
                var fieldInfo = fieldInfos[index];
                var clr = (Color) fieldInfo.GetValue(typeof(Color));
                ColorName[fieldInfo.Name] = clr;
                ColorName[fieldInfo.Name.ToLower()] = clr;
                ColorHex[clr] = clr.ToRgba().ToString("X");
                if (clr != Color.Transparent) Colors[index] = clr;
            }

            Icons = new Dictionary<string, MapIconsIndex>(200);
            foreach (var icon in Enum.GetValues(typeof(MapIconsIndex))) Icons[icon.ToString()] = (MapIconsIndex) icon;
        }


        public static Color GetRandomColor(this Color c) => Colors[MathHepler.Randomizer.Next(0, Colors.Length - 1)];

        public static MapIconsIndex IconIndexByName(string name) {
            Icons.TryGetValue(name, out var result);
            return result;
        }

        public static Color GetColorByName(string name) => ColorName.TryGetValue(name, out var result) ? result : Color.Zero;

        public static string Hex(this Color clr) => ColorHex.TryGetValue(clr, out var result) ? result : ColorHex[Color.Transparent];

        public static uint ToImgui(this Color c) => (uint) c.ToRgba();

        public static System.Numerics.Vector4 ToImguiVec4(this Color c) =>
            new System.Numerics.Vector4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);

        public static System.Numerics.Vector4 ToImguiVec4(this Color c, byte alpha) => new System.Numerics.Vector4(c.R, c.G, c.B, alpha);
        public static System.Numerics.Vector4 ToVector4Num(this Vector4 v) => new System.Numerics.Vector4(v.X, v.Y, v.Z, v.W);

        public static System.Numerics.Vector2 ToVector2Num(this Vector2 v) => new System.Numerics.Vector2(v.X, v.Y);

        public static Color ToSharpColor(this System.Numerics.Vector4 v) => new Color(v.X, v.Y, v.Z, v.W);

        public static int GetOffset<T>(string name) where T : struct {
            try
            {
                var type = typeof(T);
                var offset = (int) type.GetFields().FirstF(x => x.Name == name).GetCustomAttributesData()
                                       .First(x => x.AttributeType.Name.Equals("FieldOffsetAttribute", StringComparison.Ordinal))
                                       .ConstructorArguments.First().Value;
                return offset;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public static ValidCache<T> ValidCache<T>(this Entity entity, Func<T> func) {
            return new ValidCache<T>(entity,func);
        }


        public static uint HexToUInt(this ReadOnlySpan<char> span) {
            uint num1 = 0;
            for (int i = 0; i < span.Length; i++)
            {
                var c = span[i];
                if (num1 > 268435455U)
                    return num1;
                num1 *= 16U;
                if (c == char.MinValue) continue;
                uint num2 = num1;
                if (c >= '0' && c <= '9')
                    num2 += c - 48U;
                else if (c >= 'A' && c <= 'F')
                    num2 += (uint) c - 65 + 10;
                else
                    num2 += (uint) c - 97 + 10;
                if (num2 < num1)
                    return num1;
                num1 = num2;
            }
            return num1;
        }
    }
}