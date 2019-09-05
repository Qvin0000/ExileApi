using System;
using Exile;
using GameOffsets;
using Shared.Interfaces;

namespace PoEMemory.Components
{
    public class Buff : RemoteMemoryObject
    {

        private BuffOffsets? _offsets;
        public BuffOffsets BuffOffsets => (BuffOffsets) (_offsets ??= M.Read<BuffOffsets>(Address));
 
        public Buff() {
            _name = new Lazy<string>(() =>
            {
                var formattableString = $"{nameof(Buff)}{BuffOffsets.Name}";
                string read;
                var tries = 0;
                do
                {
                    read = Cache.StringCache.Read(formattableString,
                                                  () => M.ReadStringU(M.Read<BuffStringOffsets>(BuffOffsets.Name).String));
                    if (read == string.Empty) Cache.StringCache.Remove(formattableString);

                    tries++;
                } while (read == string.Empty && tries < 7);

                return read;
            });
        }

        private Lazy<string> _name;

        public string Name => _name.Value;

        public byte Charges => M.Read<byte>(Address + 44);

        //public int SkillId => M.Read<int>(Address + 0x5C); // I think this is part of another structure referenced in a pointer at 0x58
        public float MaxTime => BuffOffsets.MaxTime; // infinity for auras and always on buff
        public float Timer => BuffOffsets.Timer;     // timeleft

        public override string ToString() => $"{Name} - Chargs: {Charges} MaxTime: {MaxTime} Timer: {Timer}";
    }
}