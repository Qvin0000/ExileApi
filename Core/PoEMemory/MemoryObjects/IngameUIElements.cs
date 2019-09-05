using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using Exile.PoEMemory.Elements;
using GameOffsets;
using PoEMemory.Elements;
using Shared.Interfaces;

namespace PoEMemory
{
    public class IngameUIElements : Element
    {
        private CachedValue<IngameUElementsOffsets> _cachedValue;
        public IngameUElementsOffsets IngameUIElementsStruct => _cachedValue.Value;

        public IngameUIElements() => _cachedValue = new FrameCache<IngameUElementsOffsets>(() => M.Read<IngameUElementsOffsets>(Address));

        private Element _gameUI;
        public Element GameUI => _gameUI ?? (_gameUI = GetObject<Element>(IngameUIElementsStruct.GameUI));
        public SellWindow SellWindow => GetObject<SellWindow>(IngameUIElementsStruct.SellWindow);
        private Element _purchaseWindow;
        public Element PurchaseWindow => _purchaseWindow ?? (_purchaseWindow = GetObject<Element>(IngameUIElementsStruct.PurchaseWindow));
        private SubterraneanChart _DelveWindow;

        public SubterraneanChart DelveWindow =>
            _DelveWindow ?? (_DelveWindow = GetObject<SubterraneanChart>(IngameUIElementsStruct.DelveWindow));

        public SkillBarElement SkillBar => GetObject<SkillBarElement>(IngameUIElementsStruct.SkillBar);
        public SkillBarElement HiddenSkillBar => GetObject<SkillBarElement>(IngameUIElementsStruct.HiddenSkillBar);
        public PoeChatElement ChatBox => GetObject<PoeChatElement>(M.Read<long>(Address + 0x3F8, 0x2C8, 0xE20, 0x350));
        public IList<string> ChatMessages => ChatBox.Children.Select(x => x.Text).ToList();
        public Element QuestTracker => GetObject<Element>(IngameUIElementsStruct.QuestTracker);
        public Element OpenLeftPanel => GetObject<Element>(IngameUIElementsStruct.OpenLeftPanel);
        public Element OpenRightPanel => GetObject<Element>(IngameUIElementsStruct.OpenRightPanel);
        public StashElement StashElement => GetObject<StashElement>(IngameUIElementsStruct.StashElement);
        public InventoryElement InventoryPanel => GetObject<InventoryElement>(IngameUIElementsStruct.InventoryPanel);
        public Element TreePanel => GetObject<Element>(IngameUIElementsStruct.TreePanel);
        public Element AtlasPanel => GetObject<Element>(IngameUIElementsStruct.AtlasPanel);
        private Map _map;
        public Map Map => _map ?? (_map = GetObject<Map>(IngameUIElementsStruct.Map));

        public ItemsOnGroundLabelElement ItemsOnGroundLabelElement =>
            GetObject<ItemsOnGroundLabelElement>(IngameUIElementsStruct.itemsOnGroundLabelRoot);

        public IList<LabelOnGround> ItemsOnGroundLabels => ItemsOnGroundLabelElement.LabelsOnGround;
        public Element GemLvlUpPanel => GetObject<Element>(IngameUIElementsStruct.GemLvlUpPanel);
        public ItemOnGroundTooltip ItemOnGroundTooltip => GetObject<ItemOnGroundTooltip>(IngameUIElementsStruct.ItemOnGroundTooltip);
        public MapStashTabElement MapStashTab => ReadObject<MapStashTabElement>(IngameUIElementsStruct.MapTabWindowStartPtr + 0xAA0);
        public Element Sulphit => GetObject<Element>(IngameUIElementsStruct.Map).GetChildAtIndex(0);
        private Cursor _cursor;
        public Cursor Cursor => _cursor ?? (_cursor = GetObject<Cursor>(IngameUIElementsStruct.Mouse));
        private Element _BetrayalWindow;
        public Element BetrayalWindow => _BetrayalWindow ?? (_BetrayalWindow = GetObject<Element>(IngameUIElementsStruct.BetrayalWindow));
        public Element SyndicateTree => GetObject<Element>(M.Read<long>(BetrayalWindow.Address + 0xA50));
        private Element _UnveilWindow;
        public Element UnveilWindow => _UnveilWindow ?? (_UnveilWindow = GetObject<Element>(IngameUIElementsStruct.UnveilWindow));
        private IncursionWindow _IncursionWindow;


        public Element ZanaMissionChoice =>
            _ZanaMissionChoice ?? (_ZanaMissionChoice = GetObject<Element>(IngameUIElementsStruct.ZanaMissionChoice));

        private Element _ZanaMissionChoice;

        public IncursionWindow IncursionWindow =>
            _IncursionWindow ?? (_IncursionWindow = GetObject<IncursionWindow>(IngameUIElementsStruct.IncursionWindow));

        private Element _SynthesisWindow;

        public Element SynthesisWindow =>
            _SynthesisWindow ?? (_SynthesisWindow = GetObject<Element>(IngameUIElementsStruct.SynthesisWindow));


        private Element _CraftBench;
        public Element CraftBench => _CraftBench ?? (_CraftBench = GetObject<Element>(IngameUIElementsStruct.CraftBenchWindow));
        public bool IsDndEnabled => M.Read<byte>(Address + 0xf92) == 1;
        public string DndMessage => M.ReadStringU(M.Read<long>(Address + 0xf98));
        public WorldMapElement AreaInstanceUi => GetObject<WorldMapElement>(IngameUIElementsStruct.AreaInstanceUi);
        public WorldMapElement WorldMap => GetObject<WorldMapElement>(IngameUIElementsStruct.WorldMap);

        public IList<Tuple<Quest, int>> GetUncompletedQuests
        {
            get
            {
                if (IngameUIElementsStruct.GetQuests == 0) return new List<Tuple<Quest, int>>();
                var stateListPres = M.ReadDoublePointerIntList(IngameUIElementsStruct.GetQuests);
                return stateListPres.Where(x => x.Item2 > 0)
                                    .Select(x => new Tuple<Quest, int>(TheGame.Files.Quests.GetByAddress(x.Item1), x.Item2)).ToList();
            }
        }

        public IList<Tuple<Quest, int>> GetCompletedQuests
        {
            get
            {
                if (IngameUIElementsStruct.GetQuests == 0) return new List<Tuple<Quest, int>>();
                var stateListPres = M.ReadDoublePointerIntList(IngameUIElementsStruct.GetQuests);
                return stateListPres.Where(x => x.Item2 == 0)
                                    .Select(x => new Tuple<Quest, int>(TheGame.Files.Quests.GetByAddress(x.Item1), x.Item2)).ToList();
            }
        }

        public Dictionary<Quest, QuestState> GetUncompletedQuests2
        {
            get
            {
                var result = new Dictionary<Quest, QuestState>();
                var keyValuePairs = GetQuestStates;
                foreach (var keyValuePair in keyValuePairs.Values)
                    if (keyValuePair.Value.QuestStateId > 0)
                        result[keyValuePair.Key] = keyValuePair.Value;

                return result;
            }
        }

        public Dictionary<string, KeyValuePair<Quest, QuestState>> GetQuestStates
        {
            get
            {
                if (IngameUIElementsStruct.GetQuests == 0) return new Dictionary<string, KeyValuePair<Quest, QuestState>>();
                var dictionary = new Dictionary<string, KeyValuePair<Quest, QuestState>>();
                foreach (var quest in GetQuests)
                {
                    if (quest == null) continue;
                    if (quest.Item1 == null) continue;

                    var value = TheGame.Files.QuestStates.GetQuestState(quest.Item1.Id, quest.Item2);

                    if (value == null) continue;

                    if (!dictionary.ContainsKey(quest.Item1.Id))
                        dictionary.Add(quest.Item1.Id, new KeyValuePair<Quest, QuestState>(quest.Item1, value));
                }

                return dictionary.OrderBy(x => x.Key).ToDictionary(Key => Key.Key, Value => Value.Value);
            }
        }


        private IList<Tuple<Quest, int>> GetQuests
        {
            get
            {
                if (IngameUIElementsStruct.GetQuests == 0) return new List<Tuple<Quest, int>>();
                var stateListPres = M.ReadDoublePointerIntList(IngameUIElementsStruct.GetQuests);
                return stateListPres.Select(x => new Tuple<Quest, int>(TheGame.Files.Quests.GetByAddress(x.Item1), x.Item2)).ToList();
            }
        }
    }
}