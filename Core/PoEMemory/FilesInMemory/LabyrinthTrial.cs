namespace PoEMemory.FilesInMemory
{
    public class LabyrinthTrial : RemoteMemoryObject
    {
        private int id = -1;
        public int Id => id != -1 ? id : id = M.Read<int>(Address + 0x10);

        public WorldArea area;

        public WorldArea Area
        {
            get
            {
                if (area == null)
                {
                    var areaPtr = M.Read<long>(Address + 0x8);
                    area = TheGame.Files.WorldAreas.GetByAddress(areaPtr);
                }

                return area;
            }
        }
    }
}