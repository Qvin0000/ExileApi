namespace ExileCore.PoEMemory.Components
{
    public class StateMachine : Component
    {
        public bool CanBeTarget => M.Read<byte>(Address + 0xA0) == 1;
        public bool InTarget => M.Read<byte>(Address + 0xA2) == 1;

        #region Harvest

        private long MachineStates => M.Read<long>(Address + 0x38);
        public int EnergyType => M.Read<int>(MachineStates + 0x0);
        public bool IsVisuallyFeeding => M.Read<bool>(MachineStates + 0x4);

        #endregion
    }
}