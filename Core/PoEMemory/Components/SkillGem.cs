using GameOffsets;
using Shared.Interfaces;

namespace PoEMemory.Components
{
    public class SkillGem : Component
    {
        private CachedValue<SkillGemOffsets> _cachedValue;
        private FrameCache<GemInformation> _cachedValue2;

        public SkillGem() {
            _cachedValue = new FrameCache<SkillGemOffsets>(() => M.Read<SkillGemOffsets>(Address));
            _cachedValue2 = new FrameCache<GemInformation>(() => M.Read<GemInformation>(_cachedValue.Value.AdvanceInformation));
        }

        public uint Level => _cachedValue.Value.Level;
        public uint TotalExpGained => _cachedValue.Value.TotalExpGained;
        public uint ExperiencePrevLevel => _cachedValue.Value.TotalExpGained;
        public uint ExperienceMaxLevel => _cachedValue.Value.ExperienceMaxLevel;
        public uint ExperienceToNextLevel => ExperienceMaxLevel - ExperiencePrevLevel;
        public int MaxLevel => _cachedValue2.Value.MaxLevel;
        public int SocketColor => _cachedValue2.Value.SocketColor;
    }
}