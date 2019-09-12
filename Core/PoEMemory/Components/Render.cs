using ExileCore.Shared.Cache;
using ExileCore.Shared.Helpers;
using GameOffsets;
using SharpDX;

namespace ExileCore.PoEMemory.Components
{
    public class Render : Component
    {
        private readonly CachedValue<RenderComponentOffsets> _cachedValue;

        public Render()
        {
            _cachedValue = new FrameCache<RenderComponentOffsets>(() => M.Read<RenderComponentOffsets>(Address));
        }

        public RenderComponentOffsets RenderStruct => _cachedValue.Value;
        public float X => Pos.X;
        public float Y => Pos.Y;
        public float Z => Pos.Z;
        public Vector3 Pos => RenderStruct.Pos;
        public Vector3 InteractCenter => Pos + Bounds / 2;
        public float Height => RenderStruct.Height > 0.01f ? RenderStruct.Height : 0f;
        public string Name => Cache.StringCache.Read($"{nameof(Render)}{RenderStruct.Name.buf}", () => RenderStruct.Name.ToString(M));
        public Vector3 Rotation => RenderStruct.Rotation;
        public Vector3 Bounds => RenderStruct.Bounds;
        public Vector3 MeshRoration => RenderStruct.Rotation;
        public float TerrainHeight => RenderStruct.Height > 0.01f ? RenderStruct.Height : 0f;
    }
}
