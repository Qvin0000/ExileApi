using System;
using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
          public struct IngameStateOffsets
    {
            [FieldOffset(0x370)] public long Data;
            [FieldOffset(0x378)] public long ServerData;
          
            [FieldOffset(0x78)] public long IngameUi;
            [FieldOffset(0x4A0)] public long UIRoot;
            [FieldOffset(0x4E8)] public long UIHover;
            [FieldOffset(0x51C)] public float UIHoverX;
            [FieldOffset(0x520 )] public float UIHoverY;
            [FieldOffset(0x4E8 )] public long UIHoverTooltip;
            [FieldOffset(0x4E0 )] public float CurentUElementPosX;
            [FieldOffset(0x4E4 )] public float CurentUElementPosY;
            [FieldOffset(0x98)] public long EntityLabelMap;
            [FieldOffset(0x568)] public int DiagnosticInfoType;
            [FieldOffset(0x798)] public long LatencyRectangle;
            [FieldOffset(0xC28)] public long FrameTimeRectangle;
            [FieldOffset(0xE70)] public long FPSRectangle;
            [FieldOffset(0x554)] public float TimeInGame;
            [FieldOffset(0x558)] public float TimeInGameF;
            [FieldOffset(0xF4C)] public int Camera;
            [FieldOffset(0x524)] public int MouseXGlobal;
            [FieldOffset(0x524)] public int MouseYGlobal;
            [FieldOffset(0x524)] public float MouseXInGame;
            [FieldOffset(0x528)] public float MouseYInGame;

        }
    
}