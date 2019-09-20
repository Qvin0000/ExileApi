using System.Runtime.InteropServices;

namespace GameOffsets
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ActorDeployedObject
    {
        [FieldOffset(0x0)] public ushort Unknown1;//4 for totems, 22 for golems
        [FieldOffset(0x2)] public ushort SkillId;
        [FieldOffset(0x4)] public ushort ObjectId;
        [FieldOffset(0x6)] public ushort Unknown2;//Always 0
    }
}
