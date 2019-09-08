using System;
using System.Runtime.InteropServices;
using GameOffsets.Native;
using SharpDX;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ActorComponentOffsets
    {
        // Commented the offsets plugins are not using.
        //[FieldOffset(0x98)] public long ActionPtr;
        [FieldOffset(0xF8)] public short ActionId;
        //[FieldOffset(0xFA)] public short TotalActionCounterA;
        //[FieldOffset(0xFC)] public int TotalActionCounterB;

        //// only works for channeling skills
        //[FieldOffset(0x100)] public float TotalTimeInAction;

        //// maybe something related to user latency
        //// i.e. when was the last request send to server.
        //[FieldOffset(0x104)] public float UnknownTimer;

        [FieldOffset(0x120)] public int AnimationId;

        //// doesn't work for channeling skills
        [FieldOffset(0x128)] public Vector2 SkillDestination;

        [FieldOffset(0x438)] public NativePtrArray HasMinionArray;
        [FieldOffset(0x440)] public NativePtrArray DeployedObjectArray;
        [FieldOffset(0x3D8)] public NativePtrArray ActorSkillsArray;
        [FieldOffset(0x408)] public NativePtrArray ActorVaalSkills;
    }
}
