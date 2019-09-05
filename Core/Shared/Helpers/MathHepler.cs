using System;
using System.Linq;
using SharpDX;

namespace Shared.Helpers
{
    public static class MathHepler
    {
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        static MathHepler() => Randomizer = new Random();

        public static Vector2 RotateVector2(Vector2 v, float angle) {
            var theta = ConvertToRadians(angle);

            var cs = Math.Cos(theta);
            var sn = Math.Sin(theta);
            var px = v.X * cs - v.Y * sn;
            var py = v.X * sn + v.Y * cs;
            return new Vector2((float) px, (float) py);
        }

        public static double ConvertToRadians(double angle) => Math.PI / 180 * angle;

        public static Vector2 NormalizeVector(Vector2 vec) {
            var length = VectorLength(vec);
            vec.X /= length;
            vec.Y /= length;
            return vec;
        }

        public static float VectorLength(Vector2 vec) => (float) Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        public static Random Randomizer { get; }

        public static double GetPolarCoordinates(this Vector2 vector, out double phi) {
            double distance = vector.Length();
            phi = Math.Acos(vector.X / distance);
            if (vector.Y < 0) phi = MathUtil.TwoPi - phi;
            return distance;
        }

        public static string GetRandomWord(int length) {
            var array = new char[length];
            for (var i = 0; i < length; i++) array[i] = CHARS[Randomizer.Next(CHARS.Length)];
            return new string(array);
        }

        public static float Max(params float[] values) {
            var max = values.First();
            for (var i = 1; i < values.Length; i++) max = Math.Max(max, values[i]);
            return max;
        }

        public static Vector2 Translate(this Vector2 vector, float dx = 0f, float dy = 0f) => new Vector2(vector.X + dx, vector.Y + dy);

        public static System.Numerics.Vector2 Translate(this System.Numerics.Vector2 vector, System.Numerics.Vector2 vector2) =>
            new System.Numerics.Vector2(vector.X + vector2.X, vector.Y + vector2.Y);

        public static System.Numerics.Vector2 Translate(this System.Numerics.Vector2 vector, float dx = 0f, float dy = 0f) =>
            new System.Numerics.Vector2(vector.X + dx, vector.Y + dy);

        public static Vector2 TranslateToNum(this System.Numerics.Vector2 vector, float dx = 0f, float dy = 0f) =>
            new Vector2(vector.X + dx, vector.Y + dy);

        public static System.Numerics.Vector2 Mult(this System.Numerics.Vector2 vector, float dx = 1f, float dy = 1f) =>
            new System.Numerics.Vector2(vector.X * dx, vector.Y * dy);

        public static Vector3 Translate(this Vector3 vector, float dx, float dy, float dz) =>
            new Vector3(vector.X + dx, vector.Y + dy, vector.Z + dz);


        public static float Distance(this Vector2 a, Vector2 b) => Vector2.Distance(a, b);

        public static float DistanceSquared(this Vector2 a, Vector2 b) => Vector2.DistanceSquared(a, b);

        public static bool PointInRectangle(this Vector2 point, RectangleF rect) =>
            point.X >= rect.X && point.Y >= rect.Y && point.X <= rect.Width && point.Y <= rect.Height;

        public static RectangleF GetDirectionsUV(double phi, double distance) {
            // could not find a better place yet
            phi += Math.PI * 0.25; // fix rotation due to projection
            if (phi > 2 * Math.PI) phi -= 2 * Math.PI;

            var xSprite = (float) Math.Round(phi / Math.PI * 4);
            if (xSprite >= 8) xSprite = 0;

            float ySprite = distance > 60 ? distance > 120 ? 2 : 1 : 0;
            var x = xSprite / 8;
            var y = ySprite / 3;
            return new RectangleF(x, y, (xSprite + 1) / 8 - x, (ySprite + 1) / 3 - y);
        }
    }
}