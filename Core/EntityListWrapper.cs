using System;
using System.Collections;
using System.Collections.Generic;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Nodes;

namespace ExileCore
{
    public class EntityCollectSettingsContainer
    {
        public Stack<Entity> Simple { get; set; }
        public Queue<uint> KeyForDelete { get; set; }
        public Dictionary<uint, Entity> EntityCache { get; set; }
        public MultiThreadManager MultiThreadManager { get; set; }
        public Func<ToggleNode> ParseServer { get; set; }
        public Func<long> EntitiesCount { get; set; }
        public uint EntitiesVersion { get; set; }
        public bool NeedUpdate { get; set; } = true;
        public ToggleNode CollectEntitiesInParallelWhenMoreThanX { get; set; }
        public DebugInformation DebugInformation { get; set; }
        public bool Break { get; set; }
        public Func<bool> ParseEntitiesInMultiThread { get; set; }
    }

    public class EntityListWrapper
    {
        private readonly CoreSettings _settings;
        private readonly int coroutineTimeWait = 100;
        private readonly Dictionary<uint, Entity> entityCache;
        private readonly GameController gameController;
        private readonly Queue<uint> keysForDelete = new Queue<uint>(24);

        // private bool needUpdate = true;
        private readonly Coroutine parallelUpdateDictionary;
        private readonly Stack<Entity> Simple = new Stack<Entity>(512);
        private readonly Coroutine updateEntity;
        private readonly EntityCollectSettingsContainer entityCollectSettingsContainer;
        private static EntityListWrapper _instance;
        public EntityListWrapper(GameController gameController, CoreSettings settings, MultiThreadManager multiThreadManager)
        {
            _instance = this;
            this.gameController = gameController;
            _settings = settings;

            //    EntitiesStack = new MultiThreadProp<Stack<Entity>>();

            entityCache = new Dictionary<uint, Entity>(1000);
            gameController.Area.OnAreaChange += AreaChanged;
            EntitiesVersion = 0;

            updateEntity =
                new Coroutine(RefreshState, new WaitTime(coroutineTimeWait), null, "Update Entity")
                    {Priority = CoroutinePriority.High, SyncModWork = true};

            var collectEntitiesDebug = new DebugInformation("Collect Entities");
            /*parallelUpdateDictionary = new Coroutine(() =>
            {
                if (!EntitiesStack.CanUpdate) return;
                collectEntitiesDebug.TickAction(() =>
                {
                    gameController.Game.IngameState.Data.EntityList.CollectEntities(EntitiesStack.Write(), entityCache, multiThreadManager,
                                                                                    settings.ParseServerEntities,
                                                                                    settings.MyMultiOrAsParallel,
                                                                                    settings.CollectEntitiesInParallelWhenMoreThanX,
                                                                                    gameController.IngameState.Data.EntitiesCount,
                                                                                    _entitiesVersion);
                    _entitiesVersion++;
                });
            }, new WaitTime(coroutineTimeWait), null, "EntitiesAsDictionary") {Priority = CoroutinePriority.High};*/
            /*parallelUpdateDictionary = new Coroutine(() =>
            {
                if (!needUpdate) return;
                collectEntitiesDebug.TickAction(() =>
                {
                    gameController.Game.IngameState.Data.EntityList.CollectEntities(Simple, keysForDelete, entityCache, multiThreadManager,
                                                                                    settings.ParseServerEntities,
                                                                                    settings.MyMultiOrAsParallel,
                                                                                    settings.CollectEntitiesInParallelWhenMoreThanX,
                                                                                    gameController.IngameState.Data.EntitiesCount,
                                                                                    EntitiesVersion);

                    EntitiesVersion++;
                    needUpdate = false;
                });
            }, new WaitTime(coroutineTimeWait), null, "EntitiesAsDictionary") {Priority = CoroutinePriority.High};*/

            entityCollectSettingsContainer = new EntityCollectSettingsContainer();
            entityCollectSettingsContainer.Simple = Simple;
            entityCollectSettingsContainer.KeyForDelete = keysForDelete;
            entityCollectSettingsContainer.EntityCache = entityCache;
            entityCollectSettingsContainer.MultiThreadManager = multiThreadManager;
            entityCollectSettingsContainer.ParseServer = () => settings.ParseServerEntities;
            entityCollectSettingsContainer.ParseEntitiesInMultiThread = () => settings.ParseEntitiesInMultiThread;
            entityCollectSettingsContainer.EntitiesCount = () => gameController.IngameState.Data.EntitiesCount;
            entityCollectSettingsContainer.EntitiesVersion = EntitiesVersion;
            entityCollectSettingsContainer.CollectEntitiesInParallelWhenMoreThanX = settings.CollectEntitiesInParallelWhenMoreThanX;
            entityCollectSettingsContainer.DebugInformation = collectEntitiesDebug;

            IEnumerator Test()
            {
                while (true)
                {
                    yield return gameController.IngameState.Data.EntityList.CollectEntities(entityCollectSettingsContainer);
                    yield return new WaitTime(1000 / settings.EntitiesUpdate);
                    parallelUpdateDictionary.UpdateTicks((uint) (parallelUpdateDictionary.Ticks + 1));
                }
            }

            parallelUpdateDictionary = new Coroutine(Test(), null, "Collect entites") {SyncModWork = true};
            UpdateCondition(1000 / settings.EntitiesUpdate);

            settings.EntitiesUpdate.OnValueChanged += (sender, i) => { UpdateCondition(1000 / i); };

            var enumValues = typeof(EntityType).GetEnumValues();
            ValidEntitiesByType = new Dictionary<EntityType, List<Entity>>(enumValues.Length);

            // NotValidEntitiesByType = new Dictionary<EntityType, List<Entity>>(enumValues.Length);
            foreach (EntityType enumValue in enumValues)
            {
                ValidEntitiesByType[enumValue] = new List<Entity>(8);
            }

            //   NotValidEntitiesByType[enumValue] = new List<Entity>(8);
            PlayerUpdate += (sender, entity) => Entity.Player = entity;
        }

        //   private MultiThreadProp<Stack<Entity>> EntitiesStack;
        public ICollection<Entity> Entities => entityCache.Values;
        public uint EntitiesVersion { get; }
        public Entity Player { get; private set; }
        public List<Entity> OnlyValidEntities { get; } = new List<Entity>(500);
        public List<Entity> NotOnlyValidEntities { get; } = new List<Entity>(500);
        public Dictionary<uint, Entity> NotValidDict { get; } = new Dictionary<uint, Entity>(500);
        public Dictionary<EntityType, List<Entity>> ValidEntitiesByType { get; }

        public void StartWork()
        {
            Core.MainRunner.Run(updateEntity);
            Core.ParallelRunner.Run(parallelUpdateDictionary);
        }

        private void UpdateCondition(int coroutineTimeWait = 50)
        {
            parallelUpdateDictionary.UpdateCondtion(new WaitTime(coroutineTimeWait));
            updateEntity.UpdateCondtion(new WaitTime(coroutineTimeWait));
        }

        public event Action<Entity> EntityAdded;
        public event Action<Entity> EntityAddedAny;
        public event Action<Entity> EntityIgnored;
        public event Action<Entity> EntityRemoved;

        private void AreaChanged(AreaInstance area)
        {
            try
            {
                entityCollectSettingsContainer.Break = true;
                var dataLocalPlayer = gameController.Game.IngameState.Data.LocalPlayer;

                if (Player == null)
                {
                    if (dataLocalPlayer.Path.StartsWith("Meta"))
                    {
                        Player = dataLocalPlayer;
                        Player.IsValid = true;
                        PlayerUpdate?.Invoke(this, Player);
                    }
                }
                else
                {
                    if (Player.Address != dataLocalPlayer.Address)
                    {
                        if (dataLocalPlayer.Path.StartsWith("Meta"))
                        {
                            Player = dataLocalPlayer;
                            Player.IsValid = true;
                            PlayerUpdate?.Invoke(this, Player);
                        }
                    }
                }

                entityCache.Clear();
                OnlyValidEntities.Clear();
                NotOnlyValidEntities.Clear();

                foreach (var e in ValidEntitiesByType)
                {
                    e.Value.Clear();
                }

                //  foreach (var e in NotValidEntitiesByType) e.Value.Clear();
            }
            catch (Exception e)
            {
                DebugWindow.LogError($"{nameof(EntityListWrapper)} -> {e}");
            }
        }

        //  public Dictionary<EntityType, List<Entity>> NotValidEntitiesByType { get; }

        private void UpdateEntityCollections()
        {
            OnlyValidEntities.Clear();
            NotOnlyValidEntities.Clear();
            NotValidDict.Clear();

            foreach (var e in ValidEntitiesByType)
            {
                e.Value.Clear();
            }

            //  foreach (var e in NotValidEntitiesByType) e.Value.Clear();
            while (keysForDelete.Count > 0)
            {
                var key = keysForDelete.Dequeue();

                if (entityCache.TryGetValue(key, out var entity))
                {
                    EntityRemoved?.Invoke(entity);
                    entityCache.Remove(key);
                }
            }

            foreach (var entity in entityCache)
            {
                var entityValue = entity.Value;

                if (entityValue.IsValid)
                {
                    OnlyValidEntities.Add(entityValue);
                    ValidEntitiesByType[entityValue.Type].Add(entityValue);
                }
                else
                {
                    NotOnlyValidEntities.Add(entityValue);
                    NotValidDict[entityValue.Id] = entityValue;

                    // NotValidEntitiesByType[entityValue.Type].Add(entityValue);
                }
            }

            /*foreach (var entity in entityCache)
            {
                var entityValue = entity.Value;
                var diff = _entitiesVersion - entityValue.Version;
                if (diff > 1)
                {
                    entityValue.IsValid = false;
                    NotOnlyValidEntities.Add(entityValue);
                    NotValidEntitiesByType[entityValue.Type].Add(entityValue);
                    /*if (diff > RemoveValue)
                    {
                        if ((int)entityValue.Type <100 || entityValue.Id >= Int32.MaxValue)
                        {
                            keysForDelete.Enqueue(entity.Key);
                        }
                    }#1#
                }
                else
                {
                    OnlyValidEntities.Add(entityValue);
                    ValidEntitiesByType[entityValue.Type].Add(entityValue);
                    entityValue.IsValid = true;
                    /*if (entityValue.Type == EntityType.Monster && !entityValue.IsAlive && !entityValue.IsLegion)
                    {
                        keysForDelete.Enqueue(entity.Key);
                    }#1#
                }
            }*/
        }

        public void RefreshState()
        {
            if (gameController.Area.CurrentArea == null /*|| !EntitiesStack.CanRead */ || entityCollectSettingsContainer.NeedUpdate ||
                !Player.IsValid)
                return;

            //   var entities = EntitiesStack.Read();
            while (Simple.Count > 0)
            {
                var entity = Simple.Pop();

                if (entity == null)
                {
                    DebugWindow.LogError($"{nameof(EntityListWrapper)}.{nameof(RefreshState)} entity is null. (Very strange).");
                    continue;
                }

                var entityId = entity.Id;
                if (entityCache.TryGetValue(entityId, out _)) continue;

                if (entityId >= int.MaxValue && !_settings.ParseServerEntities)
                    continue;

                if ( /*!entity.IsValid ||*/ entity.Type == EntityType.Error)
                    continue;

                /*if (entity.Type == EntityType.Monster && (entity.GetComponent<Life>() == null ||
                                                          entity.GetComponent<ObjectMagicProperties>() == null))
                {
                    entity.IsValid = false;
                    continue;
                }*/

                if (entity.League == LeagueType.Legion)
                {
                    if (entity.Stats == null)
                        continue;
                }

                EntityAddedAny?.Invoke(entity);
                if ((int) entity.Type >= 100) EntityAdded?.Invoke(entity);

                entityCache[entityId] = entity;
            }

            UpdateEntityCollections();
            entityCollectSettingsContainer.NeedUpdate = true;
        }

        public event EventHandler<Entity> PlayerUpdate;

        public static Entity GetEntityById(uint id)
        {
            return _instance.entityCache.TryGetValue(id, out var result) ? result : null;
        }

        public string GetLabelForEntity(Entity entity)
        {
            var hashSet = new HashSet<long>();
            var entityLabelMap = gameController.Game.IngameState.EntityLabelMap;
            var num = entityLabelMap;

            while (true)
            {
                hashSet.Add(num);
                if (gameController.Memory.Read<long>(num + 0x10) == entity.Address) break;

                num = gameController.Memory.Read<long>(num);
                if (hashSet.Contains(num) || num == 0 || num == -1) return null;
            }

            return gameController.Game.ReadObject<EntityLabel>(num + 0x18).Text;
        }
    }
}
