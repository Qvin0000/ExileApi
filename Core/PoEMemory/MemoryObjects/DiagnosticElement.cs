using System;
using ExileCore.Shared.Cache;
using GameOffsets;
using ProcessMemoryUtilities.Memory;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class DiagnosticElement : RemoteMemoryObject
    {
        private readonly CachedValue<DiagnosticElementOffsets> _cachedValue;
        private readonly CachedValue<DiagnosticElementArrayOffsets> _cachedValue2;
        private readonly FrameCache<float[]> Values;

        public DiagnosticElement()
        {
            _cachedValue = new FrameCache<DiagnosticElementOffsets>(() => M.Read<DiagnosticElementOffsets>(Address));

            _cachedValue2 =
                new FrameCache<DiagnosticElementArrayOffsets>(
                    () => M.Read<DiagnosticElementArrayOffsets>(DiagnosticElementStruct.DiagnosticArray));

            Values = new FrameCache<float[]>(() =>
            {
                var buffer = new float[80];
                ProcessMemory.ReadProcessMemoryArray(M.OpenProcessHandle, (IntPtr) DiagnosticElementStruct.DiagnosticArray, buffer, 0, 80);
                return buffer;
            });
        }

        private DiagnosticElementOffsets DiagnosticElementStruct => _cachedValue.Value;
        private DiagnosticElementArrayOffsets DiagnosticElementArrayStruct => _cachedValue2.Value;
        public long DiagnosticArray => DiagnosticElementStruct.DiagnosticArray;
        public float[] DiagnosticArrayValues => Values.Value;
        public float CurrValue => DiagnosticElementArrayStruct.CurrValue;
        public int X => DiagnosticElementStruct.X;
        public int Y => DiagnosticElementStruct.Y;
        public int Width => DiagnosticElementStruct.Width;
        public int Height => DiagnosticElementStruct.Height;
    }
}
