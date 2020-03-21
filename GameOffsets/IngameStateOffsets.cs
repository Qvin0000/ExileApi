using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct IngameStateOffsets
    {
        [FieldOffset(0x378)] public long Data;
        [FieldOffset(0x380)] public long ServerData;
        [FieldOffset(0x80)] public long IngameUi;
        [FieldOffset(0x4A8)] public long UIRoot;
        [FieldOffset(0x4F0)] public long UIHover;
        [FieldOffset(0x524)] public float UIHoverX;
        [FieldOffset(0x528)] public float UIHoverY;
        [FieldOffset(0x4E8)] public long UIHoverTooltip;
        [FieldOffset(0x4E0)] public float CurentUElementPosX;
        [FieldOffset(0x4E4)] public float CurentUElementPosY;
        [FieldOffset(0x98)] public long EntityLabelMap;
        [FieldOffset(0x570)] public int DiagnosticInfoType;
        [FieldOffset(0x7A0)] public long LatencyRectangle;
        [FieldOffset(0xC40)] public long FrameTimeRectangle;
        [FieldOffset(0xE90)] public long FPSRectangle;
        [FieldOffset(0x554)] public float TimeInGame;
        [FieldOffset(0x558)] public float TimeInGameF;
        [FieldOffset(0xFE4)] public int Camera;
        [FieldOffset(0x518)] public int MouseXGlobal;
        [FieldOffset(0x51C)] public int MouseYGlobal;
        [FieldOffset(0x524)] public float MouseXInGame;
        [FieldOffset(0x528)] public float MouseYInGame;
    }
}
