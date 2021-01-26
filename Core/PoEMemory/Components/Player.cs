using System;
using System.Collections;
using System.Collections.Generic;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Enums;
using SharpDX;

namespace ExileCore.PoEMemory.Components
{
    public class Player : Component
    {
        public string PlayerName => NativeStringReader.ReadString(Address + 0x158, M);
        public uint XP => Address != 0 ? M.Read<uint>(Address + 0x17C) : 0;
        public int Strength => Address != 0 ? M.Read<int>(Address + 0x180) : 0;
        public int Dexterity => Address != 0 ? M.Read<int>(Address + 0x184) : 0;
        public int Intelligence => Address != 0 ? M.Read<int>(Address + 0x188) : 0;
        public int Level => Address != 0 ? M.Read<byte>(Address + 0x1A8) : 1;
        public int AllocatedLootId => Address != 0 ? M.Read<byte>(Address + 0x16C) : 1;
        public int HideoutLevel => M.Read<byte>(Address + 0x38E);
        public byte PropheciesCount => M.Read<byte>(Address + 0x20A);

        public IList<ProphecyDat> Prophecies
        {
            get
            {
                var result = new List<ProphecyDat>();
                var readAddr = Address + 0x20C;

                for (var i = 0; i < 7; i++)
                {
                    var prophecyId = M.Read<ushort>(readAddr);

                    //if(prophacyId > 0)//Dunno why it will never be 0 even if there is no prophecy
                    {
                        var prophecy = TheGame.Files.Prophecies.GetProphecyById(prophecyId);

                        // if (prophecy != null)
                        result.Add(prophecy);
                    }

                    readAddr += 4; //prophecy prophecyId(UShort), Skip index(byte), Skip unknown(byte)
                }

                return result;
            }
        }

        public HideoutWrapper Hideout => ReadObject<HideoutWrapper>(Address + 0x368);
        public PantheonGod PantheonMinor => (PantheonGod) M.Read<byte>(Address + 0x1AB);
        public PantheonGod PantheonMajor => (PantheonGod) M.Read<byte>(Address + 0x1AC);

        private IList<PassiveSkill> AllocatedPassivesM()
        {
            var result = new List<PassiveSkill>();
            var passiveIds = TheGame.IngameState.ServerData.PassiveSkillIds;

            foreach (var id in passiveIds)
            {
                var passive = TheGame.Files.PassiveSkills.GetPassiveSkillByPassiveId(id);

                if (passive == null)
                {
                    DebugWindow.LogMsg($"Can't find passive with id: {id}", 10, Color.Red);
                    continue;
                }

                result.Add(passive);
            }

            return result;
        }

        #region Trials

        public bool IsTrialCompleted(string trialId)
        {
            var trialWrapper = TheGame.Files.LabyrinthTrials.GetLabyrinthTrialByAreaId(trialId);

            if (trialWrapper == null)
            {
                throw new ArgumentException(
                    $"Trial with id '{trialId}' is not found. Use WorldArea.Id or LabyrinthTrials.LabyrinthTrialAreaIds[]");
            }

            return TrialPassStates.Get(trialWrapper.Id - 1);
        }

        public bool IsTrialCompleted(LabyrinthTrial trialWrapper)
        {
            if (trialWrapper == null)
                throw new ArgumentException($"Argument {nameof(trialWrapper)} should not be null");

            return TrialPassStates.Get(trialWrapper.Id - 1);
        }

        public bool IsTrialCompleted(WorldArea area)
        {
            if (area == null)
                throw new ArgumentException($"Argument {nameof(area)} should not be null");

            var trialWrapper = TheGame.Files.LabyrinthTrials.GetLabyrinthTrialByArea(area);

            if (trialWrapper == null)
                throw new ArgumentException($"Can't find trial wrapper for area '{area.Name}' (seems not a trial area).");

            return TrialPassStates.Get(trialWrapper.Id - 1);
        }

        [HideInReflection]
        private BitArray TrialPassStates
        {
            get
            {
                var stateBuff = M.ReadBytes(Address + 0x2B4, 36); // (286+) bytes of info.
                return new BitArray(stateBuff);
            }
        }

        #region Debug things

        public IList<TrialState> TrialStates
        {
            get
            {
                var result = new List<TrialState>();
                var passStates = TrialPassStates;

                foreach (var trialAreaId in LabyrinthTrials.LabyrinthTrialAreaIds)
                {
                    var wrapper = TheGame.Files.LabyrinthTrials.GetLabyrinthTrialByAreaId(trialAreaId);

                    result.Add(
                        new TrialState {TrialAreaId = trialAreaId, TrialArea = wrapper, IsCompleted = passStates.Get(wrapper.Id - 1)});
                }

                return result;
            }
        }

        public class TrialState
        {
            public LabyrinthTrial TrialArea { get; internal set; }
            public string TrialAreaId { get; internal set; }
            public bool IsCompleted { get; internal set; }
            public string AreaAddr => TrialArea.Address.ToString("x");

            public override string ToString()
            {
                return $"Completed: {IsCompleted}, Trial {TrialArea.Area.Name}, AreaId: {TrialArea.Id}";
            }
        }

        #endregion

        #endregion
    }
}
