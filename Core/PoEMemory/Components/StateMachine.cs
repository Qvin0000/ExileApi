using System;

namespace PoEMemory.Components
{
    public class StateMachine : Component
    {
        public bool CanBeTarget => M.Read<byte>(Address + 0xA0) == 1;
        public bool InTarget => M.Read<byte>(Address + 0xA2) == 1;
    }
}