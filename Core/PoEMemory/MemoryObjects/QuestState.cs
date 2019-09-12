namespace ExileCore.PoEMemory.MemoryObjects
{
    public class QuestState : RemoteMemoryObject
    {
        //public string Id { get; internal set; }
        public long QuestPtr => M.Read<long>(Address + 0x8);
        public Quest Quest => TheGame.Files.Quests.GetByAddress(QuestPtr);
        public int QuestStateId => M.Read<int>(Address + 0x10);
        public int TestOffset => M.Read<int>(Address + 0x14);
        public string QuestStateText => M.ReadStringU(M.Read<long>(Address + 0x2c));
        public string QuestProgressText => M.ReadStringU(M.Read<long>(Address + 0x34));

        public override string ToString()
        {
            return $"Id: {QuestStateId}, Quest.Id: {Quest.Id}, ProgressText {QuestProgressText}, QuestName: {Quest.Name}";
        }
    }
}
