using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.PoEMemory.FilesInMemory.Metamorph;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Static;

namespace ExileCore.PoEMemory
{
    public class FilesContainer
    {
        private readonly IMemory _memory;
        private BaseItemTypes _baseItemTypes;
        private UniversalFileWrapper<BetrayalChoiceAction> _betrayalChoiceActions;
        private UniversalFileWrapper<BetrayalChoice> _betrayalChoises;
        private UniversalFileWrapper<BetrayalDialogue> _betrayalDialogue;
        private UniversalFileWrapper<BetrayalJob> _betrayalJobs;
        private UniversalFileWrapper<BetrayalRank> _betrayalRanks;
        private UniversalFileWrapper<BetrayalReward> _betrayalRewards;
        private UniversalFileWrapper<BetrayalTarget> _betrayalTargets;
        private ModsDat _mods;
        private StatsDat _stats;
        private TagsDat _tags;
        private UniversalFileWrapper<AtlasNode> atlasNodes;
        public FilesFromMemory FilesFromMemory;
        private LabyrinthTrials labyrinthTrials;
        private MonsterVarieties monsterVarieties;
        private PassiveSkills passiveSkills;
        private PropheciesDat prophecies;
        private Quests quests;
        private QuestStates questStates;
        
        //Will be loaded on first access:
        private WorldAreas worldAreas;

        public FilesContainer(IMemory memory)
        {
            _memory = memory;
            ItemClasses = new ItemClasses();
            FilesFromMemory = new FilesFromMemory(_memory);

            using (new PerformanceTimer("Load files from memory"))
            {
                AllFiles = FilesFromMemory.GetAllFiles();
            }

            /*Task.Run(() =>
            {
                using (new PerformanceTimer("Preload stats and mods"))
                {
                    var test = Stats.records.Count;
                    var test2 = Mods.records.Count;
                    ParseFiles(AllFiles);
                }
            });*/
        }

        public ItemClasses ItemClasses { get; }
        public BaseItemTypes BaseItemTypes =>
            _baseItemTypes ?? (_baseItemTypes = new BaseItemTypes(_memory, () => FindFile("Data/BaseItemTypes.dat")));
        public ModsDat Mods => _mods ?? (_mods = new ModsDat(_memory, () => FindFile("Data/Mods.dat"), Stats, Tags));
        public StatsDat Stats => _stats ?? (_stats = new StatsDat(_memory, () => FindFile("Data/Stats.dat")));
        public TagsDat Tags => _tags ?? (_tags = new TagsDat(_memory, () => FindFile("Data/Tags.dat")));
        public WorldAreas WorldAreas => worldAreas ?? (worldAreas = new WorldAreas(_memory, () => FindFile("Data/WorldAreas.dat")));
        public PassiveSkills PassiveSkills =>
            passiveSkills ?? (passiveSkills = new PassiveSkills(_memory, () => FindFile("Data/PassiveSkills.dat")));
        public LabyrinthTrials LabyrinthTrials =>
            labyrinthTrials ?? (labyrinthTrials = new LabyrinthTrials(_memory, () => FindFile("Data/LabyrinthTrials.dat")));
        public Quests Quests => quests ?? (quests = new Quests(_memory, () => FindFile("Data/Quest.dat")));
        public QuestStates QuestStates => questStates ?? (questStates = new QuestStates(_memory, () => FindFile("Data/QuestStates.dat")));
        public MonsterVarieties MonsterVarieties =>
            monsterVarieties ?? (monsterVarieties = new MonsterVarieties(_memory, () => FindFile("Data/MonsterVarieties.dat")));
        public PropheciesDat Prophecies => prophecies ?? (prophecies = new PropheciesDat(_memory, () => FindFile("Data/Prophecies.dat")));
        public UniversalFileWrapper<AtlasNode> AtlasNodes =>
            atlasNodes ?? (atlasNodes = new AtlasNodes(_memory, () => FindFile("Data/AtlasNode.dat")));
        public UniversalFileWrapper<BetrayalTarget> BetrayalTargets =>
            _betrayalTargets ?? (_betrayalTargets =
                new UniversalFileWrapper<BetrayalTarget>(_memory, () => FindFile("Data/BetrayalTargets.dat")));
        public UniversalFileWrapper<BetrayalJob> BetrayalJobs =>
            _betrayalJobs ?? (_betrayalJobs = new UniversalFileWrapper<BetrayalJob>(_memory, () => FindFile("Data/BetrayalJobs.dat")));
        public UniversalFileWrapper<BetrayalRank> BetrayalRanks =>
            _betrayalRanks ?? (_betrayalRanks = new UniversalFileWrapper<BetrayalRank>(_memory, () => FindFile("Data/BetrayalRanks.dat")));
        public UniversalFileWrapper<BetrayalReward> BetrayalRewards =>
            _betrayalRewards ?? (_betrayalRewards =
                new UniversalFileWrapper<BetrayalReward>(_memory, () => FindFile("Data/BetrayalTraitorRewards.dat")));
        public UniversalFileWrapper<BetrayalChoice> BetrayalChoises =>
            _betrayalChoises ?? (_betrayalChoises =
                new UniversalFileWrapper<BetrayalChoice>(_memory, () => FindFile("Data/BetrayalChoices.dat")));
        public UniversalFileWrapper<BetrayalChoiceAction> BetrayalChoiceActions =>
            _betrayalChoiceActions ?? (_betrayalChoiceActions =
                new UniversalFileWrapper<BetrayalChoiceAction>(_memory, () => FindFile("Data/BetrayalChoiceActions.dat")));
        public UniversalFileWrapper<BetrayalDialogue> BetrayalDialogue =>
            _betrayalDialogue ?? (_betrayalDialogue =
                new UniversalFileWrapper<BetrayalDialogue>(_memory, () => FindFile("Data/BetrayalDialogue.dat")));

        #region Metamorph

        private UniversalFileWrapper<MetamorphMetaSkill> _metamorphMetaSkills;
        private UniversalFileWrapper<MetamorphMetaSkillType> _metamorphMetaSkillTypes;
        private UniversalFileWrapper<MetamorphMetaMonster> _metamorphMetaMonsters;
        private UniversalFileWrapper<MetamorphRewardType> _metamorphRewardTypes;
        private UniversalFileWrapper<MetamorphRewardTypeItemsClient> _metamorphRewardTypeItemsClient;

        public UniversalFileWrapper<MetamorphMetaSkill> MetamorphMetaSkills =>
            _metamorphMetaSkills ?? (_metamorphMetaSkills =
                new UniversalFileWrapper<MetamorphMetaSkill>(_memory, () => FindFile("Data/MetamorphosisMetaSkills.dat")));

        public UniversalFileWrapper<MetamorphMetaSkillType> MetamorphMetaSkillTypes =>
            _metamorphMetaSkillTypes ?? (_metamorphMetaSkillTypes =
                new UniversalFileWrapper<MetamorphMetaSkillType>(_memory, () => FindFile("Data/MetamorphosisMetaSkillTypes.dat")));

        public UniversalFileWrapper<MetamorphMetaMonster> MetamorphMetaMonsters =>
            _metamorphMetaMonsters ?? (_metamorphMetaMonsters =
                new UniversalFileWrapper<MetamorphMetaMonster>(_memory, () => FindFile("Data/MetamorphosisMetaMonsters.dat")));

        public UniversalFileWrapper<MetamorphRewardType> MetamorphRewardTypes =>
            _metamorphRewardTypes ?? (_metamorphRewardTypes =
                new UniversalFileWrapper<MetamorphRewardType>(_memory, () => FindFile("Data/MetamorphosisRewardTypes.dat")));

        public UniversalFileWrapper<MetamorphRewardTypeItemsClient> MetamorphRewardTypeItemsClient =>
            _metamorphRewardTypeItemsClient ?? (_metamorphRewardTypeItemsClient =
                new UniversalFileWrapper<MetamorphRewardTypeItemsClient>(_memory, () => FindFile("Data/MetamorphosisRewardTypeItemsClient.dat")));

        #endregion

        public Dictionary<string, FileInformation> AllFiles { get; private set; }
        public Dictionary<string, FileInformation> Metadata { get; } = new Dictionary<string, FileInformation>();
        public Dictionary<string, FileInformation> Data { get; private set; } = new Dictionary<string, FileInformation>();
        public Dictionary<string, FileInformation> OtherFiles { get; } = new Dictionary<string, FileInformation>();
        public Dictionary<string, FileInformation> LoadedInThisArea { get; private set; } = new Dictionary<string, FileInformation>(1024);
        public Dictionary<int, List<KeyValuePair<string, FileInformation>>> GroupedByTest2 { get; set; }
        public Dictionary<int, List<KeyValuePair<string, FileInformation>>> GroupedByChangeAction { get; set; }

        public void LoadFiles()
        {
            AllFiles = FilesFromMemory.GetAllFilesSync();
        }

        public event EventHandler<Dictionary<string, FileInformation>> LoadedFiles;

        public void ParseFiles(Dictionary<string, FileInformation> files)
        {
            foreach (var file in files)
            {
                if (file.Key[0] == 'M' && file.Key[8] == '/')
                    Metadata[file.Key] = file.Value;
                else if (file.Key[0] == 'D' && file.Key[4] == '/' && file.Key.EndsWith(".dat"))
                    Data[file.Key] = file.Value;
                else
                    OtherFiles[file.Key] = file.Value;
            }

            Data = Data.OrderBy(x => x.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public void ParseFiles(int gameAreaChangeCount)
        {
            if (AllFiles != null)
            {
                LoadedInThisArea = new Dictionary<string, FileInformation>(1024);

                foreach (var file in AllFiles)
                {
                    if (file.Value.ChangeCount == gameAreaChangeCount) LoadedInThisArea[file.Key] = file.Value;

                    if (file.Key[0] == 'M' && file.Key[8] == '/')
                        Metadata[file.Key] = file.Value;
                    else if (file.Key[0] == 'D' && file.Key[4] == '/' && file.Key.EndsWith(".dat"))
                        Data[file.Key] = file.Value;
                    else
                        OtherFiles[file.Key] = file.Value;
                }

                /*Task.Run(() =>
                {
                        GroupedByTest2 = Files.GroupBy(x => x.Value.Test2).OrderBy(x=>x.Key).ToDictionary(z => z.Key, w => w.ToList());
                         GroupedByChangeAction = Files.GroupBy(x => x.Value.ChangeCount ).OrderBy(x=>x.Key).ToDictionary(z => z.Key, w => w.ToList());
                });
          */

                LoadedFiles?.Invoke(this, LoadedInThisArea);
            }
        }

        public long FindFile(string name)
        {
            try
            {
                if (AllFiles.TryGetValue(name, out var result))
                    return result.Ptr;
            }
            catch (KeyNotFoundException)
            {
                const string MESSAGE_FORMAT = "Couldn't find the file in memory: {0}\nTry to restart the game.";
                MessageBox.Show(string.Format(MESSAGE_FORMAT, name), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            return 0;
        }

        #region Bestiary

        private BestiaryCapturableMonsters bestiaryCapturableMonsters;
        public BestiaryCapturableMonsters BestiaryCapturableMonsters =>
            bestiaryCapturableMonsters != null
                ? bestiaryCapturableMonsters
                : bestiaryCapturableMonsters =
                    new BestiaryCapturableMonsters(_memory, () => FindFile("Data/BestiaryCapturableMonsters.dat"));
        private UniversalFileWrapper<BestiaryRecipe> bestiaryRecipes;
        public UniversalFileWrapper<BestiaryRecipe> BestiaryRecipes =>
            bestiaryRecipes != null
                ? bestiaryRecipes
                : bestiaryRecipes = new UniversalFileWrapper<BestiaryRecipe>(_memory, () => FindFile("Data/BestiaryRecipes.dat"));
        private UniversalFileWrapper<BestiaryRecipeComponent> bestiaryRecipeComponents;
        public UniversalFileWrapper<BestiaryRecipeComponent> BestiaryRecipeComponents =>
            bestiaryRecipeComponents != null
                ? bestiaryRecipeComponents
                : bestiaryRecipeComponents =
                    new UniversalFileWrapper<BestiaryRecipeComponent>(_memory, () => FindFile("Data/BestiaryRecipeComponent.dat"));
        private UniversalFileWrapper<BestiaryGroup> bestiaryGroups;
        public UniversalFileWrapper<BestiaryGroup> BestiaryGroups =>
            bestiaryGroups != null
                ? bestiaryGroups
                : bestiaryGroups = new UniversalFileWrapper<BestiaryGroup>(_memory, () => FindFile("Data/BestiaryGroups.dat"));
        private UniversalFileWrapper<BestiaryFamily> bestiaryFamilies;
        public UniversalFileWrapper<BestiaryFamily> BestiaryFamilies =>
            bestiaryFamilies != null
                ? bestiaryFamilies
                : bestiaryFamilies = new UniversalFileWrapper<BestiaryFamily>(_memory, () => FindFile("Data/BestiaryFamilies.dat"));
        private UniversalFileWrapper<BestiaryGenus> bestiaryGenuses;
        public UniversalFileWrapper<BestiaryGenus> BestiaryGenuses =>
            bestiaryGenuses != null
                ? bestiaryGenuses
                : bestiaryGenuses = new UniversalFileWrapper<BestiaryGenus>(_memory, () => FindFile("Data/BestiaryGenus.dat"));

        #endregion
    }
}
