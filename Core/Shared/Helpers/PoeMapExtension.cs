using System;
using SharpDX;

namespace Shared.Helpers
{
    public static class PoeMapExtension
    {
        private const float MarsEllipticOrbit = 0.092f;
        private const float Offset = 5.434783f;

        public static Vector2 GridToWorld(this Vector2 v) =>
            new Vector2(v.X / MarsEllipticOrbit + Offset, v.Y / MarsEllipticOrbit + Offset);

        public static Vector2 WorldToGrid(this Vector3 v) =>
            new Vector2((float) Math.Floor(v.X * MarsEllipticOrbit), (float) Math.Floor(v.Y * MarsEllipticOrbit));

        public static Vector2 WorldToGrid(this Vector2 v) =>
            new Vector2((float) Math.Floor(v.X * MarsEllipticOrbit), (float) Math.Floor(v.Y * MarsEllipticOrbit));
    }
}