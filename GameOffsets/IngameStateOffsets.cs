using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct IngameStateOffsets
    {
		[FieldOffset(0x80)] public long IngameUi;
		[FieldOffset(0xA0)] public long EntityLabelMap;
		[FieldOffset(0x478)] public long Data;
        [FieldOffset(0x480)] public long ServerData;
        [FieldOffset(0x5A8)] public long UIRoot;
		[FieldOffset(0x5E0)] public long UIHoverTooltip;
		[FieldOffset(0x5E8)] public float CurentUElementPosX;
		[FieldOffset(0x5EC)] public float CurentUElementPosY;
		[FieldOffset(0x5F0)] public long UIHover;
		[FieldOffset(0x618)] public int MouseXGlobal;
		[FieldOffset(0x61C)] public int MouseYGlobal;
		[FieldOffset(0x624)] public float UIHoverX;
		[FieldOffset(0x628)] public float UIHoverY;
		[FieldOffset(0x62C)] public float MouseXInGame;
		[FieldOffset(0x630)] public float MouseYInGame;
		[FieldOffset(0x654)] public float TimeInGame;
		[FieldOffset(0x65C)] public float TimeInGameF;
		[FieldOffset(0x670)] public int DiagnosticInfoType;
        [FieldOffset(0x8A0)] public long LatencyRectangle;
        [FieldOffset(0xD40)] public long FrameTimeRectangle;
        [FieldOffset(0xF90)] public long FPSRectangle;
        [FieldOffset(0x10E0)] public int Camera;
    }
}
