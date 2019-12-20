namespace ExileCore.PoEMemory.FilesInMemory.Metamorph
{
    public class MetamorphRewardTypeItemsClient : RemoteMemoryObject
    {
        public MetamorphRewardType RewardType => TheGame.Files.MetamorphRewardTypes.GetByAddress(M.Read<long>(Address + 0x8));
        public int Unknown => M.Read<int>(Address + 0x10);
        public string Description => M.ReadStringU(M.Read<long>(Address + 0x14), 255);
        public override string ToString()
        {
            return $"{RewardType.Id}: {Description}";
        }
    }
}
