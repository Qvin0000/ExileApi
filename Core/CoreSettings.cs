using System.Collections.Generic;
using System.Windows.Forms;
using ExileCore.RenderQ;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace ExileCore
{
    public class CoreSettings : ISettings
    {
        [Menu("Refresh area")]
        public ButtonNode RefreshArea { get; set; } = new ButtonNode();
        [Menu("Show debug window")]
        public ToggleNode ShowDebugWindow { get; set; } = new ToggleNode(false);
        [Menu("List profiles", "Currently not works. Soon.")]
        public ListNode Profiles { get; set; } = new ListNode {Values = new List<string> {"global"}, Value = "global"};
        [Menu("Current Menu Theme")]
        public ListNode Theme { get; set; } = new ListNode {Value = ThemeEditor.DefaultThemeName};
        [Menu("Key Settings", "Tooltip", 0)]
        public EmptyNode KeyMenuRoot { get; set; } = new EmptyNode();
        [Menu("Main Menu Key Toggle", 10, 0)]
        public HotkeyNode MainMenuKeyToggle { get; set; } = Keys.F12;
        [Menu("Load plugins in multithread",
            "When you use a lot plugins that option can help hud faster start. Currently not recommend use it because can be unstable start.", 15, 0)]
        public ToggleNode MultiThreadLoadPlugins { get; set; } = new ToggleNode(false);

        //Now not using
        [Menu("Area change multi-threading", "", 20, 0)]
        [IgnoreMenu]
        public ToggleNode AreaChangeMultiThread { get; set; } = new ToggleNode(true);
        [Menu("Added entities multi-threading", "Just for test, most of plugin dont have expensive logic for turn on that option.", 31, 0)]
        public ToggleNode AddedMultiThread { get; set; } = new ToggleNode(false);
        [Menu("Coroutine Multi Thread", "", 32, 0)]
        public ToggleNode CoroutineMultiThreading { get; set; } = new ToggleNode(false);
        [Menu("Parse Entities Multi Thread", "", 33, 0)]
        public ToggleNode ParseEntitiesInMultiThread { get; set; } = new ToggleNode(false);
        [Menu("Debug information", "With this option you can check how much every plugin works.", 35, 0)]
        public ToggleNode CollectDebugInformation { get; set; } = new ToggleNode(true);
        [Menu("Threads count", "How much threads use for prepare work.", 40, 0)]
        public RangeNode<int> Threads { get; set; } = new RangeNode<int>(1, 0, 4);
        [Menu("Message if plugin render work more than X ms")]
        public RangeNode<int> CriticalTimeForPlugin { get; set; } = new RangeNode<int>(100, 1, 2000);
        [Menu("Performance", "", 100)]
        public EmptyNode PerfomanceRoot { get; set; } = new EmptyNode();
        [Menu("Target FPS", 10, 100)]
        public RangeNode<int> TargetFps { get; set; } = new RangeNode<int>(60, 5, 200);
        [Menu("Target Parallel Coroutine Fps", 11, 100)]
        public RangeNode<int> TargetParallelFPS { get; set; } = new RangeNode<int>(60, 30, 500);
        [Menu("Entites FPS", "How need often update entities. You can see in DebugWindow->Coroutines time what spent for that work.", 20,
            100)]
        public RangeNode<int> EntitiesUpdate { get; set; } = new RangeNode<int>(60, 5, 200);
        [Menu("Log read memory errors", 30, 100)]
        public ToggleNode LogReadMemoryError { get; set; } = new ToggleNode(false);
        [Menu("Dynamic FPS", "Hud FPS like FPS game", 15, 100)]
        public ToggleNode DynamicFPS { get; set; } = new ToggleNode(false);
        [Menu("Percent from game FPS", 16, 100)]
        public RangeNode<int> DynamicPercent { get; set; } = new RangeNode<int>(100, 1, 150);
        [Menu("Minimal FPS when dynamic", 17, 100)]
        public RangeNode<int> MinimalFpsForDynamic { get; set; } = new RangeNode<int>(60, 10, 150);
        [Menu("Parse server entities", 50, 100)]
        public ToggleNode ParseServerEntities { get; set; } = new ToggleNode(false);
        [Menu("Collect entities in parallel when more than X", 55, 100)]
        public ToggleNode CollectEntitiesInParallelWhenMoreThanX { get; set; } = new ToggleNode(false);
        [Menu("Limit draw plot in ms",
            "Don't put small value, because plot need a lot triangles and DebugWindow with a lot plot will be broke.")]
        public RangeNode<float> LimitDrawPlot { get; set; } = new RangeNode<float>(0.2f, 0.05f, 20f);
        [Menu("HUD VSync")]
        public ToggleNode VSync { get; set; } = new ToggleNode(false);
        [Menu("Font")]
        public ListNode Font { get; set; } = new ListNode {Values = new List<string> {"Not found"}};
        [Menu("Font size", "Currently not works. Because this option broke calculate how much pixels needs for render.")]
        [IgnoreMenu]
        public RangeNode<int> FontSize { get; set; } = new RangeNode<int>(13, 7, 36);
        [Menu("Debug log")]
        public ToggleNode ShowDebugLog { get; set; } = new ToggleNode(false);
        public ToggleNode ShowDemoWindow { get; set; } = new ToggleNode(false);
        public RangeNode<int> Volume { get; set; } = new RangeNode<int>(100, 0, 100);
        public ToggleNode Enable { get; set; } = new ToggleNode(true);
        public ToggleNode ForceForeground { get; set; } = new ToggleNode(false);
    }
}
