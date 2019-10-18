namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;
    using SharpDX;
    using GameOffsets.Native;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Positioned
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;
        [FieldOffset(0x0058)] public byte Reaction;
        [FieldOffset(0x00EC)] public Vector2i CurrentGridPosition;
        [FieldOffset(0x00F4)] public float Rotation;
        [FieldOffset(0x0118)] public Vector2 CurrentWorldPosition;
    }
}

// Amount of data in this component is a lot, but HUD don't really need all of it.
// So commenting all of it with explaination. If you want some offset, just copypaste it
// in the struct above.
/*    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Positioned
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;

        // It's just a pointer to:
        //   0x00: PathFindingComponent @ offset 0x488.
        //   0x08: RenderComponent @ offset 0x30.
        // So it's useless to us.
        [FieldOffset(0x0018)] public NativePtrArray OtherRelatedComponentsPtr;
        [FieldOffset(0x0058)] public byte Reaction; // Used for isHostile by &ing it with 0x7F.

        // Game is using an algorithm to move to player from point A to point B.
        // Let's say the algorithm calculated 3 stops from point A to point B.
        // i.e. A->X->Y->Z->B. Lets say player is somewhere between X, Y.
        // GridLastStop = X;
        // GridNextStop = Y;
        // GridCurrentPosition = Current Location, can be any Vector2i between X & Y.
        [FieldOffset(0x0058)] public Vector2i GridLastStop;
        [FieldOffset(0x0068)] public byte IsGridNextStopXYSwapped;
        //[FieldOffset(0x0069)] public byte Unknown0;
        //[FieldOffset(0x006C)] public int Unknown1;

        // -1, 1 for -X, X direction
        [FieldOffset(0x0074)] public int MovementDirectionX;
        // -1, 1 for -Y, Y direction
        [FieldOffset(0x0078)] public int MovementDirectionY;

        //[FieldOffset(0x0080)] public int Unknown1;
        //[FieldOffset(0x0084)] public int Unknown2;
        [FieldOffset(0x008C)] public Vector2i GridNextStop;
        // Lets not use it, cuz we have to use IsGridNextStopXYSwapped to swap it.
        // There is a better one below (with samen name) with exactly the same values and doesn't require swapping.
        //[FieldOffset(0x0098)] public Vector2i GridCurrentPosition;
        //[FieldOffset(0x00A8)] public double Unknown3;
        //[FieldOffset(0x00B0)] public float Unknown4;
        //[FieldOffset(0x00B4)] public float Unknown5;

        [FieldOffset(0x00B8)] public Vector2 WorldLastStop;
        [FieldOffset(0x00C4)] public Vector2 WorldNextStop;

        //[FieldOffset(0x00D0)] public Vector2 WorldDistanceBetweenLastAndNext; //useless
        [FieldOffset(0x00DC)] public float Unknown6;
        [FieldOffset(0x00E0)] public float Unknown7;
        [FieldOffset(0x00E4)] public float Unknown8;
        [FieldOffset(0x00EC)] public Vector2i GridCurrentPosition;
        [FieldOffset(0x00F4)] public float Rotation;

        // Change this to 2 if you are using zoomhack ;)
        [FieldOffset(0x0100)] public float Height;
        [FieldOffset(0x0104)] public float Size; //Entity Size in the world
        [FieldOffset(0x0108)] public float Width;
        [FieldOffset(0x0118)] public Vector2 CurrentWorldPosition;
    }
}*/