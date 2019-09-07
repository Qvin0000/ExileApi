using System;
using System.Runtime.InteropServices;
using GameOffsets.Native;
using SharpDX;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ElementOffsets
    {
        public const int OffsetBuffers = 0x6EC;

       // [FieldOffset(0x0)] public int vTable;
        /* [FieldOffset(0x3c + OffsetBuffers)] public long ChildStart;
        [FieldOffset(0x44 + OffsetBuffers)] public long ChildEnd;
        [FieldOffset(0x94 + OffsetBuffers)] public bool IsVisibleLocal;
        [FieldOffset(0xC4 + OffsetBuffers)] public long Root;
        [FieldOffset(0xCC + OffsetBuffers)] public long Parent;
        [FieldOffset(0xD4 + OffsetBuffers)] public float X;
        [FieldOffset(0xD8 + OffsetBuffers)] public float Y;
        [FieldOffset(0x104 + OffsetBuffers)] public long Tooltip;
        [FieldOffset(0x1D0 + OffsetBuffers)] public float Scale;
        [FieldOffset(0x21C  + OffsetBuffers)] public float Width;
        [FieldOffset(0x220  + OffsetBuffers)] public float Height;
        [FieldOffset(0x264 + OffsetBuffers)] public bool isHighlighted; */
        /* 3.5 
        [FieldOffset(0x3c)] public long ChildStart;
        [FieldOffset(0x44)] public long ChildEnd;
        [FieldOffset(0x94)] public bool IsVisibleLocal;
        [FieldOffset(0xC4)] public long Root;
        [FieldOffset(0xCC)] public long Parent;
        [FieldOffset(0xD4)] public float X;
        [FieldOffset(0xD8)] public float Y;
        [FieldOffset(0x104)] public long Tooltip;
        [FieldOffset(0x1D0)] public float Scale;
        [FieldOffset(0x21C )] public float Width;
        [FieldOffset(0x220 )] public float Height;
        [FieldOffset(0x264)] public bool isHighlighted;
        */

        [FieldOffset(0x18)] public long SelfPointer; //Usefull for valid check
        [FieldOffset(0x38)] public long ChildStart;
        [FieldOffset(0x38)] public NativePtrArray Childs;
        [FieldOffset(0x40)] public long ChildEnd;
        [FieldOffset(0x111)] public byte IsVisibleLocal;
        [FieldOffset(0x88)] public long Root;
        [FieldOffset(0x90)] public long Parent; //0x1C0 work only for items
        [FieldOffset(0x98)] public Vector2 Position;
        [FieldOffset(0x98)] public float X;
        [FieldOffset(0x9C)] public float Y;
       // [FieldOffset(0x338)] public long Tooltip;
        [FieldOffset(0x108)] public float Scale;
        [FieldOffset(0x130)] public float Width;
        [FieldOffset(0x134)] public float Height;
        [FieldOffset(0x178)] public bool isHighlighted; 
      //  [FieldOffset(0x3CB)] public byte isShadow; //0
      //  [FieldOffset(0x3C9)] public byte isShadow2; //1

      //  [FieldOffset(0x3B0)] public NativeStringU TestString;
    }
}