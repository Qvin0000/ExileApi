using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.Shared.Interfaces;
using SharpDX;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class LabyrinthData : RemoteMemoryObject
    {
        //we gonna use this to have identical references to rooms
        internal static Dictionary<long, LabyrinthRoom> CachedRoomsDictionary;

        public IList<LabyrinthRoom> Rooms
        {
            get
            {
                var firstPtr = M.Read<long>(Address);
                var lastPtr = M.Read<long>(Address + 0x8);

                var result = new List<LabyrinthRoom>();
                CachedRoomsDictionary = new Dictionary<long, LabyrinthRoom>();

                var roomIndex = 0;

                for (var addr = firstPtr; addr < lastPtr; addr += 0x60)
                {
                    DebugWindow.LogMsg($"Room {roomIndex} Addr: {addr.ToString("x")}", 0, Color.White);
                    if (addr == 0) continue;
                    var room = new LabyrinthRoom(M, addr, TheGame.Files.WorldAreas) {Id = roomIndex++};
                    result.Add(room);
                    CachedRoomsDictionary.Add(addr, room);
                }

                return result;
            }
        }

        internal static LabyrinthRoom GetRoomById(long roomId)
        {
            CachedRoomsDictionary.TryGetValue(roomId, out var room);
            return room;
        }
    }

    public class LabyrinthRoom
    {
        private readonly long Address;
        private readonly IMemory M;

        internal LabyrinthRoom(IMemory m, long address, WorldAreas filesWorldAreas)
        {
            FilesWorldAreas = filesWorldAreas;
            M = m;
            Address = address;
            Secret1 = ReadSecret(M.Read<long>(Address + 0x40));
            Secret2 = ReadSecret(M.Read<long>(Address + 0x50));
            Section = ReadSection(M.Read<long>(Address + 0x30));

            var roomsPointers = M.ReadPointersArray(Address, Address + 0x20);
            Connections = roomsPointers.Select(x => x == 0 ? null : LabyrinthData.GetRoomById(x)).ToArray();
        }

        public WorldAreas FilesWorldAreas { get; }
        public int Id { get; internal set; }
        public LabyrinthSecret Secret1 { get; internal set; }
        public LabyrinthSecret Secret2 { get; internal set; }
        public LabyrinthRoom[] Connections { get; internal set; } // Length is always 5
        public LabyrinthSection Section { get; internal set; }

        internal LabyrinthSection ReadSection(long addr)
        {
            if (addr == 0) return null; //Should never happens
            var section = new LabyrinthSection(M, addr, FilesWorldAreas);

            return section;
        }

        private LabyrinthSecret ReadSecret(long addr)
        {
            var secretAddress = M.Read<long>(addr);
            if (addr == 0) return null;

            var result = new LabyrinthSecret
            {
                SecretName = M.ReadStringU(M.Read<long>(addr)), Name = M.ReadStringU(M.Read<long>(addr + 0x8))
            };

            return result;
        }

        public override string ToString()
        {
            var linked = "";

            var realConnections = Connections.Where(r => r != null).ToList();

            if (realConnections.Count > 0)
                linked = $"LinkedWith: {string.Join(", ", realConnections.Select(x => x.Id.ToString()).ToArray())}";

            return $"Id: {Id}, " +
                   $"Secret1: {(Secret1 == null ? "None" : Secret1.SecretName)}, Secret2: {(Secret2 == null ? "None" : Secret2.SecretName)}, {linked}, Section: {Section}";
        }

        public class LabyrinthSecret
        {
            public string SecretName { get; internal set; }
            public string Name { get; internal set; }

            public override string ToString()
            {
                return SecretName;
            }
        }

        public class LabyrinthSection
        {
            internal LabyrinthSection(IMemory M, long addr, WorldAreas filesWorldAreas)
            {
                FilesWorldAreas = filesWorldAreas;
                SectionType = M.ReadStringU(M.Read<long>(addr + 0x8, 0));

                var overridesCount = M.Read<int>(addr + 0x5c);
                var overridesArrayPtr = M.Read<long>(addr + 0x64);

                var overridePointers = M.ReadSecondPointerArray_Count(overridesArrayPtr, overridesCount);

                for (var i = 0; i < overridesCount; i++)
                {
                    var newOverride = new LabyrinthSectionOverrides();
                    var overrideAddr = overridePointers[i];
                    newOverride.OverrideName = M.ReadStringU(M.Read<long>(overrideAddr));
                    newOverride.Name = M.ReadStringU(M.Read<long>(overrideAddr + 0x8));
                    Overrides.Add(newOverride);
                }

                SectionAreas = new LabyrinthSectionAreas(FilesWorldAreas);
                var areasPtr = M.Read<long>(addr + 0x4c);
                SectionAreas.Name = M.ReadStringU(M.Read<long>(areasPtr));

                var normalCount = M.Read<int>(areasPtr + 0x8);
                var normalArrayPtr = M.Read<long>(areasPtr + 0x10);
                SectionAreas.NormalAreasPtrs = M.ReadSecondPointerArray_Count(normalArrayPtr, normalCount);

                var cruelCount = M.Read<int>(areasPtr + 0x18);
                var cruelArrayPtr = M.Read<long>(areasPtr + 0x20);
                SectionAreas.CruelAreasPtrs = M.ReadSecondPointerArray_Count(cruelArrayPtr, cruelCount);

                var mercCount = M.Read<int>(areasPtr + 0x28);
                var mercArrayPtr = M.Read<long>(areasPtr + 0x30);
                SectionAreas.MercilesAreasPtrs = M.ReadSecondPointerArray_Count(mercArrayPtr, mercCount);

                var endgameCount = M.Read<int>(areasPtr + 0x38);
                var endgameArrayPtr = M.Read<long>(areasPtr + 0x40);
                SectionAreas.EndgameAreasPtrs = M.ReadSecondPointerArray_Count(endgameArrayPtr, endgameCount);
            }

            public WorldAreas FilesWorldAreas { get; }
            public string SectionType { get; internal set; }
            public IList<LabyrinthSectionOverrides> Overrides { get; internal set; } = new List<LabyrinthSectionOverrides>();
            public LabyrinthSectionAreas SectionAreas { get; internal set; }

            public override string ToString()
            {
                var overrides = "";

                if (Overrides.Count > 0)
                    overrides = $"Overrides: {string.Join(", ", Overrides.Select(x => x.ToString()).ToArray())}";

                return $"SectionType: {SectionType}, {overrides}";
            }
        }
    }

    public class LabyrinthSectionAreas
    {
        private List<WorldArea> cruelAreas;
        private List<WorldArea> endgameAreas;
        private List<WorldArea> mercilesAreas;
        private List<WorldArea> normalAreas;

        public LabyrinthSectionAreas(WorldAreas filesWorldAreas)
        {
            FilesWorldAreas = filesWorldAreas;
            NormalAreasPtrs = new List<long>();
            CruelAreasPtrs = new List<long>();
            MercilesAreasPtrs = new List<long>();
            EndgameAreasPtrs = new List<long>();
        }

        public WorldAreas FilesWorldAreas { get; }
        public string Name { get; set; }
        public IList<long> NormalAreasPtrs { get; set; }
        public IList<long> CruelAreasPtrs { get; set; }
        public IList<long> MercilesAreasPtrs { get; set; }
        public IList<long> EndgameAreasPtrs { get; set; }

        public IList<WorldArea> NormalAreas
        {
            get
            {
                if (normalAreas == null)
                    normalAreas = NormalAreasPtrs.Select(x => FilesWorldAreas.GetByAddress(x)).ToList();

                return normalAreas;
            }
        }

        public IList<WorldArea> CruelAreas
        {
            get
            {
                if (cruelAreas == null)
                    cruelAreas = CruelAreasPtrs.Select(x => FilesWorldAreas.GetByAddress(x)).ToList();

                return cruelAreas;
            }
        }

        public IList<WorldArea> MercilesAreas
        {
            get
            {
                if (mercilesAreas == null)
                    mercilesAreas = MercilesAreasPtrs.Select(x => FilesWorldAreas.GetByAddress(x)).ToList();

                return mercilesAreas;
            }
        }

        public IList<WorldArea> EndgameAreas
        {
            get
            {
                if (endgameAreas == null)
                    endgameAreas = EndgameAreasPtrs.Select(x => FilesWorldAreas.GetByAddress(x)).ToList();

                return endgameAreas;
            }
        }
    }

    public class LabyrinthSectionOverrides
    {
        public string Name { get; internal set; }
        public string OverrideName { get; internal set; }

        public override string ToString()
        {
            return OverrideName;
        }
    }
}
