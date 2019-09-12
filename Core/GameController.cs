using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.SomeMagic;
using SharpDX;

namespace ExileCore
{
    public class PluginBridge
    {
        private readonly Dictionary<string, object> methods = new Dictionary<string, object>();

        public T GetMethod<T>(string name) where T : class
        {
            if (methods.TryGetValue(name, out var result)) return result as T;

            return null;
        }

        public void SaveMethod(string name, object method)
        {
            methods[name] = method;
        }
    }

    public class GameController : IDisposable
    {
        private readonly CoreSettings _settings;
        private readonly DebugInformation debClearCache;
        private readonly DebugInformation debDeltaTime;
        private readonly TimeCache<Vector2> LeftCornerMap;
        private readonly TimeCache<Vector2> UnderCornerMap;
        private bool IsForeGroundLast;
        public PluginBridge PluginBridge;

        public GameController(Memory memory, SoundController soundController, SettingsContainer settings,
            MultiThreadManager multiThreadManager)
        {
            _settings = settings.CoreSettings;
            Memory = memory;
            SoundController = soundController;
            Settings = settings;
            MultiThreadManager = multiThreadManager;

            try
            {
                Cache = new Cache();
                Game = new TheGame(memory, Cache);
                Area = new AreaController(Game);
                Window = new GameWindow(memory.Process);
                Files = Game.Files;
                EntityListWrapper = new EntityListWrapper(this, _settings, multiThreadManager);
            }
            catch (Exception e)
            {
                DebugWindow.LogError(e.ToString());
            }

            PluginBridge = new PluginBridge();

            IsForeGroundCache = WinApi.IsForegroundWindow(Window.Process.MainWindowHandle);
            var values = Enum.GetValues(typeof(IconPriority));

            LeftPanel = new PluginPanel(GetLeftCornerMap());
            UnderPanel = new PluginPanel(GetUnderCornerMap());

            var debParseFile = new DebugInformation("Parse files", false);
            debClearCache = new DebugInformation("Clear cache", false);

            // Core.DebugInformations.Add(debParseFile);
            /*Area.OnAreaChange += controller =>
            {

                debParseFile.TickAction(() =>
                {
                    Files.LoadFiles();
                });
            };*/

            debDeltaTime = Core.DebugInformations.FirstOrDefault(x => x.Name == "Delta Time");

            NativeMethods.LogError = _settings.LogReadMemoryError;

            _settings.LogReadMemoryError.OnValueChanged +=
                (obj, b) => NativeMethods.LogError = _settings.LogReadMemoryError;

            LeftCornerMap = new TimeCache<Vector2>(GetLeftCornerMap, 500);
            UnderCornerMap = new TimeCache<Vector2>(GetUnderCornerMap, 500);

            eIsForegroundChanged += b =>
            {
                if (b)
                {
                    Core.MainRunner.ResumeCoroutines(Core.MainRunner.Coroutines);
                    Core.ParallelRunner.ResumeCoroutines(Core.ParallelRunner.Coroutines);
                }
                else
                {
                    Core.MainRunner.PauseCoroutines(Core.MainRunner.Coroutines);
                    Core.ParallelRunner.PauseCoroutines(Core.ParallelRunner.Coroutines);
                }

                // DebugWindow.LogMsg($"Foreground: {b}");
            };

            _settings.RefreshArea.OnPressed += () => { Area.ForceRefreshArea(_settings.AreaChangeMultiThread); };
            Area.RefreshState();
            EntityListWrapper.StartWork();
            Initialized = true;
        }

        private Stopwatch sw { get; } = Stopwatch.StartNew();
        public long ElapsedMs => sw.ElapsedMilliseconds;
        public TheGame Game { get; }
        public AreaController Area { get; }
        public GameWindow Window { get; }
        public IngameState IngameState => Game.IngameState;
        public FilesContainer Files { get; }
        public Entity Player => EntityListWrapper.Player;
        public bool IsForeGroundCache { get; set; }
        public bool InGame { get; private set; }
        public bool IsLoading { get; private set; }
        public PluginPanel LeftPanel { get; }
        public PluginPanel UnderPanel { get; }
        public IMemory Memory { get; }
        public SoundController SoundController { get; }
        public SettingsContainer Settings { get; }
        public MultiThreadManager MultiThreadManager { get; }
        public EntityListWrapper EntityListWrapper { get; }
        public Cache Cache { get; set; }
        public double DeltaTime => debDeltaTime.Tick;
        public bool Initialized { get; }
        public ICollection<Entity> Entities => EntityListWrapper.Entities;
        public Dictionary<string, object> Debug { get; } = new Dictionary<string, object>();

        public void Dispose()
        {
            Memory?.Dispose();
        }

        public static event Action<bool> eIsForegroundChanged = delegate { };

        public void Tick()
        {
            try
            {
                if (IsForeGroundLast != IsForeGroundCache)
                {
                    IsForeGroundLast = IsForeGroundCache;
                    eIsForegroundChanged(IsForeGroundCache);
                }

                AreaInstance.CurrentHash = Game.CurrentAreaHash;
                if (LeftPanel.Used) LeftPanel.StartDrawPoint = LeftCornerMap.Value;
                if (UnderPanel.Used) UnderPanel.StartDrawPoint = UnderCornerMap.Value;

                //Every 3 frame check area change and force garbage collect every new area
                if (Core.FramesCount % 3 == 0 && Area.RefreshState())
                    debClearCache.TickAction(() => { RemoteMemoryObject.Cache.TryClearCache(); });

                InGame = Game.InGame; //Game.IngameState.InGame;
                IsLoading = Game.IsLoading;
                if (InGame) CachedValue.Latency = Game.IngameState.CurLatency;
            }
            catch (Exception e)
            {
                DebugWindow.LogError(e.ToString());
            }
        }

        public Vector2 GetLeftCornerMap()
        {
            if (!InGame) return Vector2.Zero;
            var ingameState = Game.IngameState;
            var clientRect = ingameState.IngameUi.Map.SmallMiniMap.GetClientRectCache;
            var diagnosticElement = ingameState.LatencyRectangle;
            var ingameUiSulphit = ingameState.IngameUi.Sulphit;

            switch (ingameState.DiagnosticInfoType)
            {
                case DiagnosticInfoType.Off:

                    if (ingameUiSulphit != null && ingameUiSulphit.IsVisibleLocal)
                        clientRect.X -= ingameUiSulphit.GetClientRectCache.Width;

                    break;
                case DiagnosticInfoType.Short:
                    clientRect.X -= diagnosticElement.X + 30;
                    break;

                case DiagnosticInfoType.Full:

                    if (ingameUiSulphit != null && ingameUiSulphit.IsVisibleLocal)
                        clientRect.X -= ingameUiSulphit.GetClientRectCache.Width;

                    clientRect.Y += diagnosticElement.Y + diagnosticElement.Height;
                    var fpsRectangle = ingameState.FPSRectangle;

                    // clientRect.X -= fpsRectangle.X + fpsRectangle.Width + 6;
                    break;
            }

            return new Vector2(clientRect.X, clientRect.Y);
        }

        private Vector2 GetUnderCornerMap()
        {
            if (!InGame) return Vector2.Zero;

            //  var questPanel = Game.IngameState.IngameUi.QuestTracker;
            var gemPanel = Game.IngameState.IngameUi.GemLvlUpPanel.Parent;

            //    var questPanelRect = questPanel.GetClientRectCache();
            RectangleF clientRect;
            clientRect = gemPanel.GetClientRectCache;
            return new Vector2(clientRect.X + clientRect.Width, clientRect.Y + clientRect.Height);
        }
    }
}
