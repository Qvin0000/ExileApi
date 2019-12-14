using System;
using System.Collections.Generic;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;
using GameOffsets.Native;
using SharpDX;

namespace ExileCore.PoEMemory.MemoryObjects
{
    public class Entity : RemoteMemoryObject
    {
        private readonly Dictionary<Type, Component> _cacheComponents = new Dictionary<Type, Component>(4);
        private readonly CachedValue<bool> _hiddenCheckCache;
        private Vector3 _boundsCenterPos = Vector3.Zero;
        private Dictionary<string, long> _cacheComponents2;
        private long? _componentLookup;
        private float _distancePlayer = float.MaxValue;
        private EntityOffsets? _entityOffsets;
        private Vector2 _gridPos = Vector2.Zero;
        private long? _id;
        private uint? _inventoryId;
        private bool _isAlive;
        private bool _isDead;
        private CachedValue<bool> _isHostile;
        private bool _isOpened;
        private bool _isTargetable;
        private string _metadata;
        private string _path;
        private Vector3 _pos = Vector3.Zero;
        private MonsterRarity? _rarity;
        private string _renderName = "Empty";
        private Dictionary<GameStat, int> _stats;
        private readonly ValidCache<List<Buff>> buffCache;
        private bool isHidden;
        private readonly object locker = new object();
        private int pathReadErrorTimes;

        public Entity()
        {
            _hiddenCheckCache = new LatancyCache<bool>(() =>
            {
                if (IsValid)
                    isHidden = HasComponent<Life>() && GetComponent<Life>().HasBuff("hidden_monster");

                return isHidden;
            }, 50);

            buffCache = this.ValidCache(() => GetComponent<Life>()?.Buffs);
        }

        public static Entity Player { get; set; }
        private static Dictionary<string, string> ComponentsName { get; } = new Dictionary<string, string>();

        public float DistancePlayer
        {
            get
            {
                if (Player == null)
                    return _distancePlayer;

                if (IsValid)
                {
                    _distancePlayer = Player.GridPos.Distance(GridPos);
                    return _distancePlayer;
                }

                _distancePlayer = Player.GridPos.Distance(GridPos);
                return _distancePlayer;
            }
        }

        public EntityOffsets EntityOffsets
        {
            get
            {
                if (_entityOffsets != null)
                    return _entityOffsets.Value;

                if (Address != 0)
                    _entityOffsets = M.Read<EntityOffsets>(Address);

                if (_entityOffsets != null) return _entityOffsets.Value;
                IsValid = false;
                return default;
            }
        }

        public EntityType Type { get; private set; }
        public LeagueType League { get; private set; } = LeagueType.General;
        public long ComponentList => EntityOffsets.ComponentList;
        public bool IsHidden => _hiddenCheckCache.Value;
        public string Debug => $"{EntityOffsets.Head.MainObject:X} List: {EntityOffsets.ComponentList:X}";
        public uint Version { get; set; }
        public bool IsValid { get; set; }

        public bool IsAlive
        {
            get
            {
                if (!IsValid)
                    return _isAlive;

                var life = GetComponent<Life>();

                if (life == null || life.OwnerAddress != Address)
                {
                    if (_distancePlayer < 70)
                        _isAlive = false;

                    return _isAlive;
                }

                _isAlive = life.CurHP > 0;
                return _isAlive;
            }
        }

        public Vector3 Pos
        {
            get
            {
                if (!IsValid)
                    return _pos;

                var render = GetComponent<Render>();

                if (render == null)
                    return _pos;

                _pos.X = render.X;
                _pos.Y = render.Y;
                _pos.Z = render.Z + render.Bounds.Z;
                return _pos;
            }
        }

        public Vector3 BoundsCenterPos
        {
            get
            {
                if (!IsValid)
                    return _boundsCenterPos;

                var render = GetComponent<Render>();

                if (render == null)
                    return _boundsCenterPos;

                _boundsCenterPos = render.InteractCenter;
                return _boundsCenterPos;
            }
        }

        public Vector2 GridPos
        {
            get
            {
                if (!IsValid)
                    return _gridPos;

                var positioned = GetComponent<Positioned>();

                if (positioned == null)
                    return _gridPos;

                if (positioned.OwnerAddress != Address)
                    return _gridPos;

                _gridPos = positioned.GridPos;
                return _gridPos;
            }
        }

        public string RenderName
        {
            get
            {
                if (!IsValid)
                    return _renderName;

                var render = GetComponent<Render>();

                if (render == null)
                    return _renderName;

                _renderName = render.Name;
                return _renderName;
            }
        }

        public MonsterRarity Rarity => (MonsterRarity) (_rarity = _rarity ?? (GetComponent<ObjectMagicProperties>()?.Rarity ?? MonsterRarity.White));

        public bool IsOpened
        {
            get
            {
                if (!IsValid)
                    return _isOpened;

                var chest = GetComponent<Chest>();

                if (chest == null)
                    return _isOpened;

                var targetable = GetComponent<Targetable>();

                if (targetable == null)
                    return _isOpened;

                _isOpened = !targetable.isTargetable || chest.IsOpened;
                return _isOpened;
            }
        }

        public bool IsDead
        {
            get
            {
                if (!IsValid)
                    return _isDead;

                _isDead = !_isAlive;
                return _isDead;
            }
        }

        public Dictionary<GameStat, int> Stats
        {
            get
            {
                if (!IsValid)
                    return _stats;

                var stats = GetComponent<Stats>();

                if (stats == null)
                    return _stats;

                if (stats.OwnerAddress != Address)
                {
                    stats = GetComponentFromMemory<Stats>();

                    if (stats.OwnerAddress != Address)
                        return _stats;
                }

                var statsStatDictionary = stats.StatDictionary;

                if (statsStatDictionary.Count == 0 && (_stats == null || _stats.Count != 0))
                    return _stats;

                _stats = statsStatDictionary;

                return _stats;
            }
        }

        public bool IsTargetable
        {
            get
            {
                if (!IsValid)
                {
                    if (_isTargetable && DistancePlayer < 100)
                        _isTargetable = false;

                    return _isTargetable;
                }

                var targetable = GetComponent<Targetable>();
                _isTargetable = targetable != null && targetable.isTargetable;
                return _isTargetable;
            }
        }

        public List<Buff> Buffs => buffCache.Value;
        private string CachePath { get; set; }

        public string Path
        {
            get
            {
                if (_path == null)
                {
                    //   _path = Cache.StringCache.Read($"{nameof(Entity)}{EntityOffsets.Head.MainObject}",() => M.Read<PathEntityOffsets>(EntityOffsets.Head.MainObject).ToString(M));
                    if (EntityOffsets.Head.MainObject == 0)
                    {
                        if (CachePath == null)
                        {
                            IsValid = false;
                            return null;
                        }

                        return CachePath;
                    }

                    var p = M.Read<PathEntityOffsets>(EntityOffsets.Head.MainObject);

                    if (p.Path.Ptr == 0)
                    {
                        if (CachePath == null)
                        {
                            IsValid = false;
                            return null;
                        }

                        return CachePath;
                    }

                    _path = Cache.StringCache.Read($"{p.Path.Ptr}{p.Length}", () => p.ToString(M));

                    //  _path = p.ToString(M);
                    //_path= M.Read<PathEntityOffsets>(EntityOffsets.Head.MainObject).ToString(M);
                    if (!_path.StartsWith("Metadata"))
                    {
                        _path = M.Read<PathEntityOffsets>(EntityOffsets.Head.MainObject).ToString(M);

                        // Cache.StringCache.Remove($"{nameof(Entity)}{EntityOffsets.Head.MainObject}");
                        Cache.StringCache.Remove($"{p.Path.Ptr}{p.Length}");
                    }

                    if (_path.Length > 0 && _path[0] != 'M')
                    {
                        pathReadErrorTimes++;
                        IsValid = false;
                        _path = null;

                        if (pathReadErrorTimes > 10)
                        {
                            _path = "ERROR PATH";
                            DebugWindow.LogError("Entity path error.");
                        }
                    }
                    else
                        CachePath = _path;
                }

                return _path;
            }
        }

        public string Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    if (Path != null)
                    {
                        var splitIndex = Path.IndexOf("@", StringComparison.Ordinal);

                        if (splitIndex != -1)
                            _metadata = Path.Substring(0, splitIndex);
                        else
                            return Path;
                    }
                }

                return _metadata;
            }
        }

        private long ComponentLookup
        {
            get
            {
                if (_componentLookup != null)
                    return _componentLookup.Value;

                return (long) (_componentLookup = M.Read<long>(Address + 0x8, 0x38, 0x30));
            }
        }

        public uint Id => (uint) (_id = _id ?? M.Read<uint>(Address + 0x50));
        public uint InventoryId => (uint) (_inventoryId = _inventoryId ?? M.Read<uint>(Address + 0x68));

        //public bool IsValid => M.Read<int>(EntityOffsets.Head.MainObject+0x18,0) == 0x65004D;
        public Dictionary<string, long> CacheComp => _cacheComponents2 ?? (_cacheComponents2 = GetComponents());
        public bool IsHostile =>
            _isHostile?.Value ?? (_isHostile = new TimeCache<bool>(() => (GetComponent<Positioned>()?.Reaction & 0x7f) != 1, 100)).Value;
        private Dictionary<Type, object> PluginData { get; } = new Dictionary<Type, object>();
        public event EventHandler<Entity> OnUpdate;

        public override string ToString()
        {
            return $"#{Id} <{Type}> ({Rarity}) {Metadata}: ({Address:X})";
        }

        public float Distance(Entity entity)
        {
            return GridPos.Distance(entity.GridPos);
        }

        protected override void OnAddressChange()
        {
            _entityOffsets = M.Read<EntityOffsets>(Address);
            _componentLookup = null;

            // _id = null;
            _inventoryId = null;
            _pos = Vector3.Zero;
            _cacheComponents.Clear();
            _cacheComponents2 = null;

            if (Type == EntityType.Error)
            {
                Type = ParseType();
                if (Type != EntityType.Error) IsValid = true;
            }

            OnUpdate?.Invoke(this, this);
        }

        public bool Check(uint entityId)
        {
            if (_id != null)
            {
                if (_id != entityId)
                {
                    DebugWindow.LogMsg($"Was ID: {Id} New ID: {entityId} To Path: {Path}", 3);
                    _id = entityId;
                    _path = null;
                    _metadata = null;
                    Type = ParseType();
                }
            }

            if (Type != EntityType.Error)
            {
                if (Type == EntityType.Effect || Type == EntityType.Daemon) return true;

                return CacheComp != null && Id == entityId && CheckRarity();
            }

            return false;
        }

        private bool CheckRarity()
        {
            return Rarity >= MonsterRarity.White && Rarity <= MonsterRarity.Unique;
        }

        public void UpdatePointer(long newAddress)
        {
            Address = newAddress;
        }

        private Dictionary<string, long> GetComponents()
        {
            lock (locker)
            {
                var result = new Dictionary<string, long>();

                // the first address is a base object that doesn't contain a component, so read the first component
                if (ComponentLookup <= 0)
                    return null;

                var node = M.Read<NativeListNodeComponent>(ComponentLookup);

                if (node.Next == 0)
                    return null;

                var lastComponent = node.Next;
                var tries = 0;

                var AllStatsParsed = false;

                while (!AllStatsParsed && tries < 3)
                {
                    if (tries > 0)
                    {
                        _componentLookup = null;

                        if (ComponentLookup <= 0)
                            return null;

                        node = M.Read<NativeListNodeComponent>(ComponentLookup);

                        if (node.Next == 0)
                        {
                            tries++;
                            AllStatsParsed = false;
                            continue;
                        }

                        lastComponent = node.Next;
                    }

                    AllStatsParsed = true;
                    var startTime = 0;
                    node = M.Read<NativeListNodeComponent>(lastComponent);

                    while (node.Next != lastComponent)
                    {
                        if (startTime > 30)
                        {
                            IsValid = false;
                            DebugWindow.LogError($"GetComponents() -> {Path} => {Debug}");
                            DebugWindow.LogError($"Component list: {ComponentList}. Node: {node}");

                            foreach (var l in result)
                            {
                                DebugWindow.LogError($"{l.Key} : {l.Value:X}");
                            }

                            return result;
                        }

                        startTime++;
                        var strPtr = node.String;

                        if (strPtr == 0)
                        {
                            AllStatsParsed = false;
                            continue;
                        }

                        var name = Cache.StringCache.Read($"{nameof(Entity)}{strPtr}", () => M.ReadString(strPtr));

                        //var name = M.ReadString(strPtr);
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            if (!ComponentsName.ContainsKey(name))
                            {
                                var arr = name.ToCharArray();
                                arr = Array.FindAll(arr, c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-');
                                ComponentsName[name] = new string(arr);
                            }
                            else
                                name = ComponentsName[name];
                        }
                        else
                        {
                            node = M.Read<NativeListNodeComponent>(node.Next);
                            AllStatsParsed = false;
                            continue;
                        }

                        if (result.ContainsKey(name)) continue;

                        var nodeComponentList = ComponentList + node.ComponentList * 8;

                        if (nodeComponentList == 0)
                        {
                            node = M.Read<NativeListNodeComponent>(node.Next);
                            AllStatsParsed = false;
                            continue;
                        }

                        var componentAddress = M.Read<long>(nodeComponentList);
                        node = M.Read<NativeListNodeComponent>(node.Next);

                        if (componentAddress <= 0x1000000 || componentAddress >= 0xF0000000000)
                        {
                            AllStatsParsed = false;
                            continue;
                        }

                        var componentOwnerAddress = M.Read<long>(componentAddress + 0x8);

                        if (componentOwnerAddress == Address)
                            result[name] = componentAddress;
                        else
                            AllStatsParsed = false;
                    }

                    tries++;
                }

                if (!AllStatsParsed)
                    IsValid = false;

                return result;
            }
        }

        public bool HasComponent<T>() where T : Component, new()
        {
            return CacheComp != null && CacheComp.TryGetValue(typeof(T).Name, out var address) && address != 0;
        }

        /*public bool HasComponent<T>() where T : Component, new()
        {
            string name = typeof(T).Name;
            long componentLookup = ComponentLookup;
            var addr = M.Read<long>(componentLookup);
            int i = 0;
            while (!M.ReadString(M.Read<long>(addr + 0x10)).Equals(name))
            {
                if (addr == 0)
                    return false;
                addr = M.Read<long>(addr);
                ++i;
                if (addr == componentLookup || addr == 0 || addr == -1 || i >= 200)
                    return false;
            }

            CacheComp[name] = M.Read<long>(ComponentList + M.Read<int>(addr+0x18)*8);
            return true;
        }*/

        public T GetComponent<T>() where T : Component, new()
        {
            if (_cacheComponents.TryGetValue(typeof(T), out var result)) return (T) result;

            if (CacheComp != null && CacheComp.TryGetValue(typeof(T).Name, out var address))
            {
                var component = GetObject<T>(address);
                _cacheComponents[typeof(T)] = component;
                return component;
            }

            return null;
        }

        public bool CheckComponentForValid<T>() where T : Component, new()
        {
            var c = GetComponent<T>();

            if (c.OwnerAddress != Address)
            {
                var componentFromMemory = GetComponentFromMemory<T>();
                if (componentFromMemory.OwnerAddress == Address) return true;

                return false;
            }

            return true;
        }

        public T GetComponentFromMemory<T>() where T : Component, new()
        {
            if (CacheComp.TryGetValue(typeof(T).Name, out var address))
            {
                var component = GetObject<T>(address);
                _cacheComponents[typeof(T)] = component;
                return component;
            }

            return null;
        }

        private EntityType ParseType()
        {
            // if (EntityOffsets.ComponentList <= 0) return EntityType.Error;

            if (string.IsNullOrEmpty(Path)) return EntityType.Error;

            if (Path.StartsWith("Metadata/Effects/", StringComparison.Ordinal)) return EntityType.Effect;

            if (Path.StartsWith("Metadata/Monsters/Daemon/", StringComparison.Ordinal)) return EntityType.Daemon;

            if (Version > 0 && Id > int.MaxValue)
                return EntityType.ServerObject;

            if (HasComponent<Chest>())
            {
                if (Path.StartsWith("Metadata/Chests/DelveChests", StringComparison.Ordinal))
                {
                    League = LeagueType.Delve;
                    return EntityType.Chest;
                }

                if (Path.StartsWith("Metadata/Chests/Incursion", StringComparison.Ordinal))
                {
                    League = LeagueType.Incursion;
                    return EntityType.Chest;
                }

                if (Path.StartsWith("Metadata/Chests/Legion", StringComparison.Ordinal))
                {
                    League = LeagueType.Legion;
                    return EntityType.Chest;
                }

                return EntityType.Chest;
            }

            if (HasComponent<NPC>() && Path.StartsWith("Metadata/NPC", StringComparison.Ordinal))
                return EntityType.Npc;

            if (HasComponent<Monster>())
            {
                if (Path.StartsWith("Metadata/Monsters/LegionLeague/", StringComparison.Ordinal))
                    League = LeagueType.Legion;

                return EntityType.Monster;
            }

            if (HasComponent<Shrine>())
                return EntityType.Shrine;

            if (HasComponent<WorldItem>())
                return EntityType.WorldItem;

            if (HasComponent<Player>())
                return EntityType.Player;

            if (HasComponent<MinimapIcon>())
            {
                if (Path.Equals("Metadata/Terrain/Missions/Hideouts/Objects/HideoutCraftingBench", StringComparison.Ordinal))
                    return EntityType.CraftUnlock;

                if (HasComponent<AreaTransition>()) return EntityType.AreaTransition;

                if (Path.EndsWith("Waypoint", StringComparison.Ordinal)) return EntityType.Waypoint;

                if (HasComponent<Portal>()) return EntityType.TownPortal;

                if (HasComponent<Monolith>()) return EntityType.Monolith;

                if (HasComponent<Transitionable>() && Path.StartsWith("Metadata/MiscellaneousObjects/Abyss"))
                {
                    League = LeagueType.Abyss;
                    return EntityType.MiscellaneousObjects;
                }

                if (Path.Equals("Metadata/Terrain/Leagues/Legion/Objects/LegionInitiator", StringComparison.Ordinal))
                    return EntityType.LegionMonolith;

                if (Path.Equals("Metadata/MiscellaneousObjects/Stash", StringComparison.Ordinal)) return EntityType.Stash;
                if (Path.Equals("Metadata/MiscellaneousObjects/GuildStash", StringComparison.Ordinal)) return EntityType.GuildStash;

                if (Path.Equals("Metadata/MiscellaneousObjects/Delve/DelveCraftingBench", StringComparison.Ordinal))
                    return EntityType.DelveCraftingBench;

                if (Path.Equals("Metadata/MiscellaneousObjects/Breach/BreachObject", StringComparison.Ordinal)) return EntityType.Breach;
                if (Path.Equals("Metadata/Terrain/Leagues/Delve/Objects/DelveMineral")) return EntityType.Resource;

                return EntityType.IngameIcon;
            }

            if (HasComponent<Portal>()) return EntityType.Portal;

            if (HasComponent<HideoutDoodad>()) return EntityType.HideoutDecoration;

            if (HasComponent<Monolith>()) return EntityType.MiniMonolith;

            if (HasComponent<ClientBetrayalChoice>()) return EntityType.BetrayalChoice;

            if (HasComponent<RenderItem>()) return EntityType.Item;

            if (Path.StartsWith("Metadata/MiscellaneousObjects/Lights", StringComparison.Ordinal)) return EntityType.Light;

            if (Path.StartsWith("Metadata/Terrain", StringComparison.Ordinal)) return EntityType.Terrain;

            if (Path.StartsWith("Metadata/Pet", StringComparison.Ordinal)) return EntityType.Pet;

            return EntityType.None;
        }

        public T GetHudComponent<T>() where T : class
        {
            if (PluginData.TryGetValue(typeof(T), out var result)) return (T) result;
            return null;
        }

        public void SetHudComponent<T>(T data)
        {
            lock (locker)
            {
                PluginData[typeof(T)] = data;
            }
        }
    }
}
