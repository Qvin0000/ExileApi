using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components
{
    public class SkillGem : Component
    {
        private readonly CachedValue<SkillGemOffsets> _cachedValue;
        private readonly FrameCache<GemInformation> _cachedValue2;

        public SkillGem()
        {
            _cachedValue = new FrameCache<SkillGemOffsets>(() => M.Read<SkillGemOffsets>(Address));
            _cachedValue2 = new FrameCache<GemInformation>(() => M.Read<GemInformation>(_cachedValue.Value.AdvanceInformation));
        }

        public int Level => (int)_cachedValue.Value.Level;//TODO: fixme, remove cast
        public uint TotalExpGained => _cachedValue.Value.TotalExpGained;
        public uint ExperiencePrevLevel => _cachedValue.Value.TotalExpGained;
        public uint ExperienceMaxLevel => _cachedValue.Value.ExperienceMaxLevel;
        public uint ExperienceToNextLevel => ExperienceMaxLevel - ExperiencePrevLevel;
        public int MaxLevel => _cachedValue2.Value.MaxLevel;
        public int SocketColor => _cachedValue2.Value.SocketColor;
    }
}
