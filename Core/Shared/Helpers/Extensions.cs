using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using JM.LinqFaster;
using SharpDX;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace ExileCore.Shared.Helpers
{
    public static class Extensions
    {
        private static readonly Color[] Colors;
        private static readonly Dictionary<string, MapIconsIndex> Icons;

        static Extensions()
        {
            var fieldInfos = typeof(Color).GetFields(BindingFlags.Public | BindingFlags.Static);

            Colors = new Color[fieldInfos.Length];
            ColorName = new Dictionary<string, Color>(fieldInfos.Length);
            ColorHex = new Dictionary<Color, string>(fieldInfos.Length);

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

            foreach (var icon in Enum.GetValues(typeof(MapIconsIndex)))
            {
                Icons[icon.ToString()] = (MapIconsIndex) icon;
            }
        }

        private static Dictionary<string, Color> ColorName { get; } = new Dictionary<string, Color>();
        private static Dictionary<Color, string> ColorHex { get; } = new Dictionary<Color, string>();

        public static Color GetRandomColor(this Color c)
        {
            return Colors[MathHepler.Randomizer.Next(0, Colors.Length - 1)];
        }

        public static MapIconsIndex IconIndexByName(string name)
        {
            Icons.TryGetValue(name, out var result);
            return result;
        }

        public static Color GetColorByName(string name)
        {
            return ColorName.TryGetValue(name, out var result) ? result : Color.Zero;
        }

        public static string Hex(this Color clr)
        {
            return ColorHex.TryGetValue(clr, out var result) ? result : ColorHex[Color.Transparent];
        }

        public static uint ToImgui(this Color c)
        {
            return (uint) c.ToRgba();
        }

        public static Vector4 ToImguiVec4(this Color c)
        {
            return new Vector4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
        }

        public static Vector4 ToImguiVec4(this Color c, byte alpha)
        {
            return new Vector4(c.R, c.G, c.B, alpha);
        }

        public static Vector4 ToVector4Num(this SharpDX.Vector4 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static Vector2 ToVector2Num(this SharpDX.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Color ToSharpColor(this Vector4 v)
        {
            return new Color(v.X, v.Y, v.Z, v.W);
        }

        public static int GetOffset<T>(string name) where T : struct
        {
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

        public static ValidCache<T> ValidCache<T>(this Entity entity, Func<T> func)
        {
            return new ValidCache<T>(entity, func);
        }

        public static uint HexToUInt(this ReadOnlySpan<char> span)
        {
            uint num1 = 0;

            for (var i = 0; i < span.Length; i++)
            {
                var c = span[i];

                if (num1 > 268435455U)
                    return num1;

                num1 *= 16U;
                if (c == char.MinValue) continue;
                var num2 = num1;

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
