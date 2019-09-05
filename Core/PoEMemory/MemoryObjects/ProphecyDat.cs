namespace PoEMemory
{
    public class ProphecyDat : RemoteMemoryObject
    {
        public int Index { get; set; }

        private string id;
        public string Id => id != null ? id : id = M.ReadStringU(M.Read<long>(Address), 255);

        private string predictionText;

        public string PredictionText =>
            predictionText != null ? predictionText : predictionText = M.ReadStringU(M.Read<long>(Address + 0x8), 255);

        public int ProphecyId => M.Read<int>(Address + 0x10);

        private string name;
        public string Name => name != null ? name : name = M.ReadStringU(M.Read<long>(Address + 0x14));

        private string flavourText;
        public string FlavourText => flavourText != null ? flavourText : flavourText = M.ReadStringU(M.Read<long>(Address + 0x1c), 255);

        public long ProphecyChainPtr => M.Read<long>(Address + 0x44);    //TODO ProphecyChainDat
        public int ProphecyChainPosition => M.Read<int>(Address + 0x4c); //TODO ProphecyChainDat

        public bool IsEnabled => M.Read<byte>(Address + 0x50) > 0;

        public int SealCost => M.Read<int>(Address + 0x51);

        public override string ToString() => $"{Name}, {PredictionText}";
    }
}