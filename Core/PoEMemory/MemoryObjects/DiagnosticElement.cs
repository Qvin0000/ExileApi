using System;
using System.Globalization;
using System.Linq;
using GameOffsets;
using ProcessMemoryUtilities.Memory;
using Shared.Interfaces;

namespace PoEMemory
{
    public class DiagnosticElement : RemoteMemoryObject
    {
        private CachedValue<DiagnosticElementOffsets> _cachedValue;
        private CachedValue<DiagnosticElementArrayOffsets> _cachedValue2;

        private DiagnosticElementOffsets DiagnosticElementStruct => _cachedValue.Value;
        private DiagnosticElementArrayOffsets DiagnosticElementArrayStruct => _cachedValue2.Value;

        public DiagnosticElement() {
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

        private FrameCache<float[]> Values;

        public long DiagnosticArray => DiagnosticElementStruct.DiagnosticArray;
        public float[] DiagnosticArrayValues => Values.Value;
        public float CurrValue => DiagnosticElementArrayStruct.CurrValue;
        public int X => DiagnosticElementStruct.X;
        public int Y => DiagnosticElementStruct.Y;
        public int Width => DiagnosticElementStruct.Width;
        public int Height => DiagnosticElementStruct.Height;
    }
}