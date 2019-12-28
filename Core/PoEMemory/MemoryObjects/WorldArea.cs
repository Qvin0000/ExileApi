using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class WorldArea : RemoteMemoryObject
    {
        private List<WorldArea> connections;
        private List<WorldArea> corruptedAreas;
        private string id;
        private string name;
        public string Id => id != null ? id : id = M.ReadStringU(M.Read<long>(Address));
        public int Index { get; set; }
        public string Name => name != null ? name : name = M.ReadStringU(M.Read<long>(Address + 8), 255);
        public int Act => M.Read<int>(Address + 0x10);
        public bool IsTown => M.Read<byte>(Address + 0x14) == 1;
        public bool HasWaypoint => M.Read<byte>(Address + 0x15) == 1;
        public int AreaLevel => M.Read<int>(Address + 0x26);
        public int WorldAreaId => M.Read<int>(Address + 0x2a);
        public bool IsAtlasMap => Id.StartsWith("MapAtlas");
        public bool IsMapWorlds => Id.StartsWith("MapWorlds");
        public bool IsCorruptedArea => Id.Contains("SideArea") || Id.Contains("Sidearea");
        public bool IsMissionArea => Id.Contains("Mission");
        public bool IsDailyArea => Id.Contains("Daily");
        public bool IsMapTrialArea => Id.StartsWith("EndGame_Labyrinth_trials");
        public bool IsLabyrinthArea => !IsMapTrialArea && Id.Contains("Labyrinth");
        public bool IsAbyssArea =>
            Id.Equals("AbyssLeague") || Id.Equals("AbyssLeague2") || Id.Equals("AbyssLeagueBoss") || Id.Equals("AbyssLeagueBoss2") ||
            Id.Equals("AbyssLeagueBoss3");
        public bool IsUnique => M.Read<bool>(Address + 0x1EC);

        public IList<WorldArea> Connections
        {
            get
            {
                if (connections == null)
                {
                    connections = new List<WorldArea>();

                    var connectionsCount = M.Read<int>(Address + 0x16);
                    var connectionsPtr = M.Read<long>(Address + 0x1e);

                    if (connectionsCount > 30)
                        return connections;

                    for (var i = 0; i < connectionsCount; i++)
                    {
                        var newArea = TheGame.Files.WorldAreas.GetByAddress(M.Read<long>(connectionsPtr));
                        connections.Add(newArea);
                        connectionsPtr += 8;
                    }
                }

                return connections;
            }
        }

        public IList<WorldArea> CorruptedAreas
        {
            get
            {
                if (corruptedAreas == null)
                {
                    corruptedAreas = new List<WorldArea>();

                    var corruptedAreasPtr = M.Read<long>(Address + 0x103);
                    var corruptedAreasCount = M.Read<int>(Address + 0xfb);

                    if (corruptedAreasCount > 30)
                        return corruptedAreas;

                    for (var i = 0; i < corruptedAreasCount; i++)
                    {
                        var newArea = TheGame.Files.WorldAreas.GetByAddress(M.Read<long>(corruptedAreasPtr));
                        corruptedAreas.Add(newArea);
                        corruptedAreasPtr += 8;
                    }
                }

                return corruptedAreas;
            }
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
