namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;
    using SharpDX;
    using GameOffsets.Native;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Render
    {
        [FieldOffset(0x0000)] public ComponentHeader Header;

        // Same as Positioned Component CurrentWorldPosition,
        // but this one contains Z axis; Z axis is where the HealthBar is.
        // If you want to use ground Z axis, swap current one with TerrainHeight.
        [FieldOffset(0x0078)] public Vector3 CurrentWorldPosition;

        // Changing this value will move the in-game healthbar up/down.
        // Not sure if it's really X,Y,Z or something else. They all move
        // healthbar up/down. This might be useless. Update PoeHUD before removing this.
        [FieldOffset(0x008C)] public Vector3 CharactorModelBounds;
        [FieldOffset(0x0098)] public NativeUnicodeText ClassName;

        // Exactly the same as provided in the Positioned component.
        // Also available @ 0x00BC with exact same value.
        // There is a one at 0x00C0 but that one is zero for NPCs and
        // have different value than others.
        //[FieldOffset(0x00B8)] public float Rotation;
        [FieldOffset(0x00E0)] public float TerrainHeight;
    }
}