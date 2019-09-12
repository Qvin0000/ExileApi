using ExileCore.Shared.Cache;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components
{
    public class Pathfinding : Component
    {
        private readonly CachedValue<PathfindingComponentOffsets> _cachedValue;

        public Pathfinding()
        {
            _cachedValue = new FrameCache<PathfindingComponentOffsets>(() => M.Read<PathfindingComponentOffsets>(Address));
        }

        private PathfindingComponentOffsets _offsets => _cachedValue.Value;
        public Vector2i TargetMovePos => _offsets.ClickToNextPosition;
        public Vector2i PreviousMovePos => _offsets.WasInThisPosition;
        public Vector2i WantMoveToPosition => _offsets.WantMoveToPosition;
        public bool IsMoving => _offsets.IsMoving == 2;
        public float StayTime => _offsets.StayTime;
    }
}
