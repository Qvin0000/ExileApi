using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;
using SharpDX;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class ServerStashTab : RemoteMemoryObject
    {
        public const int StructSize = 0x40;
        private const int ColorOffset = 0x2c;
        private readonly CachedValue<ServerStashTabOffsets> _cachedValue;

        public ServerStashTab()
        {
            _cachedValue = new FrameCache<ServerStashTabOffsets>(() => M.Read<ServerStashTabOffsets>(Address));
        }

        public ServerStashTabOffsets ServerStashTabOffsets => _cachedValue.Value;
        public string NameOld => NativeStringReader.ReadString(Address + 0x8, M) + (RemoveOnly ? " (Remove-only)" : string.Empty);
        public string Name => ServerStashTabOffsets.Name.ToString(M);

        //public int InventoryId => M.Read<int>(Address + 0x20);
        public uint Color => ServerStashTabOffsets.Color;
        public Color Color2 =>
            new Color(M.Read<byte>(Address + ColorOffset), M.Read<byte>(Address + ColorOffset + 1),
                M.Read<byte>(Address + ColorOffset + 2));
        public InventoryTabPermissions MemberFlags => (InventoryTabPermissions) ServerStashTabOffsets.MemberFlags;
        public InventoryTabPermissions OfficerFlags => (InventoryTabPermissions) ServerStashTabOffsets.OfficerFlags;
        public InventoryTabType TabType => (InventoryTabType) ServerStashTabOffsets.TabType;
        public ushort VisibleIndex => ServerStashTabOffsets.DisplayIndex;

        //public ushort LinkedParentId => M.ReadUShort(Address + 0x26);
        public InventoryTabFlags Flags => (InventoryTabFlags) ServerStashTabOffsets.Flags;
        public bool RemoveOnly => (Flags & InventoryTabFlags.RemoveOnly) == InventoryTabFlags.RemoveOnly;
        public bool IsHidden => (Flags & InventoryTabFlags.Hidden) == InventoryTabFlags.Hidden;

        public override string ToString()
        {
            return $"{Name}, DisplayIndex: {VisibleIndex}, {TabType}";
        }
    }
}
