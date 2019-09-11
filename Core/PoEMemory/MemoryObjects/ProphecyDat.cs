namespace ExileCore.PoEMemory.MemoryObjects
{
    public class ProphecyDat : RemoteMemoryObject
    {
        private string flavourText;
        private string id;
        private string name;
        private string predictionText;
        public int Index { get; set; }
        public string Id => id != null ? id : id = M.ReadStringU(M.Read<long>(Address), 255);
        public string PredictionText =>
            predictionText != null ? predictionText : predictionText = M.ReadStringU(M.Read<long>(Address + 0x8), 255);
        public int ProphecyId => M.Read<int>(Address + 0x10);
        public string Name => name != null ? name : name = M.ReadStringU(M.Read<long>(Address + 0x14));
        public string FlavourText => flavourText != null ? flavourText : flavourText = M.ReadStringU(M.Read<long>(Address + 0x1c), 255);
        public long ProphecyChainPtr => M.Read<long>(Address + 0x44); //TODO ProphecyChainDat
        public int ProphecyChainPosition => M.Read<int>(Address + 0x4c); //TODO ProphecyChainDat
        public bool IsEnabled => M.Read<byte>(Address + 0x50) > 0;
        public int SealCost => M.Read<int>(Address + 0x51);

        public override string ToString()
        {
            return $"{Name}, {PredictionText}";
        }
    }
}
