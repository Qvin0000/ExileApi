using Shared.Structs;
using GameOffsets;
using Shared.Interfaces;
using SharpDX;

namespace PoEMemory.Components
{
    public class Pathfinding : Component
    {
        private CachedValue<PathfindingComponentOffsets> _cachedValue;
        private PathfindingComponentOffsets _offsets => _cachedValue.Value;

        public Pathfinding() =>
            _cachedValue = new FrameCache<PathfindingComponentOffsets>(() => M.Read<PathfindingComponentOffsets>(Address));

        public Vector2i TargetMovePos => _offsets.ClickToNextPosition;
        public Vector2i PreviousMovePos => _offsets.WasInThisPosition;
        public Vector2i WantMoveToPosition => _offsets.WantMoveToPosition;
        public bool IsMoving => _offsets.IsMoving == 2;
        public float StayTime => _offsets.StayTime;
    }
}