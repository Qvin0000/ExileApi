using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components
{
    public class StateMachine : Component
    {
        private readonly CachedValue<StateMachineComponentOffsets> _stateMachine;

        public StateMachine()
        {
            _stateMachine = new FrameCache<StateMachineComponentOffsets>(() =>
                Address == 0 ? default : M.Read<StateMachineComponentOffsets>(Address));
        }

        public IList<StateMachineState> States => ReadStates();
        public bool CanBeTarget => M.Read<byte>(Address + 0xA0) == 1;
        public bool InTarget => M.Read<byte>(Address + 0xA2) == 1;

        [Obsolete("Use ReadStates() instead")] private long MachineStates => M.Read<long>(Address + 0x38);
        [Obsolete("Use ReadStates() instead")] public int EnergyType => M.Read<int>(MachineStates + 0x0);
        [Obsolete("Use ReadStates() instead")] public bool IsVisuallyFeeding => M.Read<bool>(MachineStates + 0x4);

        public IList<StateMachineState> ReadStates()
        {
            var offsets = _stateMachine.Value;
            var size = offsets.StatesValues.Size;
            var statesCount = size / 8;
            var result = new List<StateMachineState>();

            if (statesCount == 0)
                return result;

            if (statesCount > 100)
            {
                Logger.Log.Error($"Error reading states in StateMachine component");
                return result;
            }

            var valueByteArray = M.ReadBytes(offsets.StatesValues.First, size);
            var valuesIntArray = new long[statesCount];

            unsafe
            {
                fixed (void* ptrSrc = valueByteArray)
                {
                    fixed (void* ptrDst = valuesIntArray)
                    {
                        Buffer.MemoryCopy(ptrSrc, ptrDst, valueByteArray.Length, valueByteArray.Length);
                    }
                }
            }


            var statesPtr = M.Read<long>(offsets.StatesPtr + 0x10);

            for (var i = 0; i < statesCount; i++)
            {
                var readAddr = statesPtr + i * 0xA8;
                var nativeStringU = M.Read<NativeUtf8Text>(readAddr);
                var stateName = nativeStringU.ToString(M);
                var stateValue = valuesIntArray[i];

                result.Add(new StateMachineState(stateName, stateValue));
            }

            return result;
        }
    }

    public class StateMachineState
    {
        public StateMachineState(string name, long value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public long Value { get; }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}