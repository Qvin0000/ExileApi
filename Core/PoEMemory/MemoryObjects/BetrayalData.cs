using System.Collections.Generic;
using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class BetrayalData : RemoteMemoryObject
    {
        public BetrayalSyndicateLeadersData SyndicateLeadersData => GetObject<BetrayalSyndicateLeadersData>(M.Read<long>(Address + 0x288));

        public List<BetrayalSyndicateState> SyndicateStates
        {
            get
            {
                var betrayalStateAddr = M.Read<long>(Address + 0x2D0);

                return M.ReadStructsArray<BetrayalSyndicateState>(betrayalStateAddr,
                    betrayalStateAddr + BetrayalSyndicateState.STRUCT_SIZE * 14,
                    BetrayalSyndicateState.STRUCT_SIZE, this);
            }
        }

        public BetrayalEventData BetrayalEventData
        {
            get
            {
                var addr = M.Read<long>(Address + 0x2E8, 0x2F0);
                return addr == 0 ? null : GetObject<BetrayalEventData>(addr);
            }
        }
    }

    public class BetrayalSyndicateLeadersData : RemoteMemoryObject
    {
        public List<BetrayalSyndicateState> Leaders =>
            new List<BetrayalSyndicateState>
            {
                ReadObjectAt<BetrayalSyndicateState>(0x0),
                ReadObjectAt<BetrayalSyndicateState>(0x8),
                ReadObjectAt<BetrayalSyndicateState>(0x10),
                ReadObjectAt<BetrayalSyndicateState>(0x18)
            };
    }

    public class BetrayalEventData : RemoteMemoryObject
    {
        public BetrayalTarget Target1 => TheGame.Files.BetrayalTargets.GetByAddress(M.Read<long>(Address + 0x2D0));
        public BetrayalTarget Target2 => TheGame.Files.BetrayalTargets.GetByAddress(M.Read<long>(Address + 0x2F0));
        public BetrayalTarget Target3 => TheGame.Files.BetrayalTargets.GetByAddress(M.Read<long>(Address + 0x300));
        public BetrayalChoiceAction Action => TheGame.Files.BetrayalChoiceActions.GetByAddress(M.Read<long>(Address + 0x2E0));
    }
}
