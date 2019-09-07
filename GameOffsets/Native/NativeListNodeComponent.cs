using System;
using System.Runtime.InteropServices;

namespace GameOffsets.Native
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct NativeListNodeComponent
    {
        [FieldOffset(0x0)] public long Next;

        [FieldOffset(0x8)] public long Prev;

        [FieldOffset(0x10)] public long String;
        [FieldOffset(0x18)] public int ComponentList;

        public override string ToString() { return $"Next: {Next} Prev: {Prev} String: {String} ComponentList: {ComponentList}"; }
    }
}