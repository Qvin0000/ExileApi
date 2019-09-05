using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Exile;
using Shared.Interfaces;
using GameOffsets;
using GameOffsets.Native;
using JM.LinqFaster;
using SharpDX;

namespace Shared.Helpers
{
    public static class MiscHelpers
    {
        public static string InsertBeforeUpperCase(this string str, string append)
        {   
            var sb = new StringBuilder();

            char previousChar = char.MinValue; // Unicode '\0'

            foreach (char c in str)
            {
                if (char.IsUpper(c))
                {
                    // If not the first character and previous character is not a space, insert a space before uppercase

                    if (sb.Length != 0 && previousChar != ' ')
                    {
                        sb.Append(append);
                    }           
                }

                sb.Append(c);

                previousChar = c;
            }

            return sb.ToString();
        }
        public static string GetTimeString(TimeSpan timeSpent) {
            var allsec = (int) timeSpent.TotalSeconds;
            var secs = allsec % 60;
            var mins = allsec / 60;
            var hours = mins / 60;
            mins = mins % 60;
            return string.Format(hours > 0 ? "{0}:{1:00}:{2:00}" : "{1}:{2:00}", hours, mins, secs);
        }

        public static string ToString(this NativeStringU str, IMemory mem) {
            if (str.Capacity >= 8)
            {
                if (str.Size < 256)
                    return mem.ReadStringU(str.buf, (int) str.Size * 2);
                return mem.ReadStringU(str.buf);
            }

            return Encoding.Unicode.GetString(BitConverter.GetBytes(str.buf).Concat(BitConverter.GetBytes(str.buf2))
                                                          .Take((int) str.Size * 2).ToArray());
        }


        public static string ToString(this PathEntityOffsets str, IMemory mem) => mem.ReadStringU(str.Path.Ptr, (int) str.Length * 2);

        public static T ToEnum<T>(this string value) => (T) Enum.Parse(typeof(T), value, true);

        public static Vector2 ClickRandom(this RectangleF clientRect, int x = 3, int y = 3) {
            var resX = MathHepler.Randomizer.Next((int) clientRect.TopLeft.X + x, (int) clientRect.TopRight.X - x);
            var resY = MathHepler.Randomizer.Next((int) clientRect.TopLeft.Y + y, (int) clientRect.BottomLeft.Y - x);
            return new Vector2(resX, resY);
        }

        public static void PerfTimerLogMsg(Action act, string msg, float time = 3f, bool log = false) {
            using (new PerformanceTimer(
                msg, 0, (s, span) => DebugWindow.LogMsg($"{s} -> {span.TotalMilliseconds} ms.", time, Color.Zero.GetRandomColor()), false))
            {
                act?.Invoke();
            }
        }
        
        public static IEnumerable<string[]> LoadConfigBase(string path, int columnsCount = 2) {
            return File.ReadAllLines(path)
                       .Where(line => !string.IsNullOrWhiteSpace(line) && line.IndexOf(';') >= 0 && !line.StartsWith("#"))
                       .Select(line => line.Split(new[] { ';' }, columnsCount).Select(parts => parts.Trim()).ToArray());
        }
    }
}