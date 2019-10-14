using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using ExileCore.RenderQ;
using ExileCore.Shared;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using ImGuiNET;
using JM.LinqFaster;
using SharpDX;
using Vector2 = System.Numerics.Vector2;

namespace ExileCore
{
    public class MenuWindow : IDisposable
    {
        private static readonly Stopwatch swStartedProgram = Stopwatch.StartNew();
        private readonly SettingsContainer _settingsContainer;
        private readonly Core core;
        private int _index = -1;
        private DebugInformation AllPlugins;
        private readonly Action CoreSettings = () => { };
        private readonly DebugInformation debugInformation;
        private bool demo_window;
        private bool firstTime = true;
        private List<DebugInformation> MainDebugs = new List<DebugInformation>();
        private Action MoreInformation;
        private List<DebugInformation> NotMainDebugs = new List<DebugInformation>();
        private readonly Action OnWindowChange;
        private Windows openWindow;
        private readonly int PluginNameWidth = 200;
        private List<PluginWrapper> plugins;
        private List<DebugInformation> PluginsDebug = new List<DebugInformation>();
        private bool refresh;
        private int selected = 0;
        private Action Selected = () => { };
        private string selectedName = "";
        private readonly Stopwatch sw = Stopwatch.StartNew();
        private readonly ThemeEditor themeEditor;
        private readonly Array WindowsName;

        public static bool IsOpened;

        public MenuWindow(Core core, SettingsContainer settingsContainer, Dictionary<string, FontContainer> fonts)
        {
            this.core = core;
            _settingsContainer = settingsContainer;
            _CoreSettings = settingsContainer.CoreSettings;
            Fonts = fonts;
            themeEditor = new ThemeEditor(_CoreSettings);
            /*Input.RegisterKey(Keys.F12);
            Input.ReleaseKey += (sender, keys) =>
            {
                if (keys== SettingsCoreSettings.MainMenuKeyToggle.Value)
                {
                    Enable = !Enable;
                }
            };*/

            CoreSettingsDrawers = new List<ISettingsHolder>();

            SettingsParser.Parse(_CoreSettings, CoreSettingsDrawers);
            Selected = CoreSettings;

            CoreSettings = () =>
            {
                foreach (var drawer in CoreSettingsDrawers)
                {
                    drawer.Draw();
                }
            };

            _index = -1;
            Selected = CoreSettings;

            Core.DebugInformations.CollectionChanged += OnDebugInformationsOnCollectionChanged;
            debugInformation = new DebugInformation("DebugWindow", false);
            OpenWindow = Windows.MainDebugs;
            WindowsName = Enum.GetValues(typeof(Windows));

            OnWindowChange += () =>
            {
                MoreInformation = null;
                selectedName = "";
            };

            Input.RegisterKey(_CoreSettings.MainMenuKeyToggle);
            _CoreSettings.MainMenuKeyToggle.OnValueChanged += () => { Input.RegisterKey(_CoreSettings.MainMenuKeyToggle); };

            _CoreSettings.Enable.OnValueChanged += (sender, b) =>
            {
                if (!_CoreSettings.Enable)
                {
                    try
                    {
                        _settingsContainer.SaveCoreSettings();

                        foreach (var plugin in core.pluginManager.Plugins)
                        {
                            try
                            {
                                _settingsContainer.SaveSettings(plugin.Plugin);
                            }
                            catch (Exception e)
                            {
                                DebugWindow.LogError($"SaveSettings for plugin error: {e}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        DebugWindow.LogError($"SaveSettings error: {e}");
                    }
                }
            };
        }

        public CoreSettings _CoreSettings { get; }
        public Dictionary<string, FontContainer> Fonts { get; }
        public List<ISettingsHolder> CoreSettingsDrawers { get; }

        private Windows OpenWindow
        {
            get => openWindow;
            set
            {
                if (openWindow != value)
                {
                    openWindow = value;
                    OnWindowChange?.Invoke();
                }
            }
        }

        public void Dispose()
        {
            Core.DebugInformations.CollectionChanged -= OnDebugInformationsOnCollectionChanged;
            _settingsContainer.SaveCoreSettings();
        }

        private void OnDebugInformationsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                if (firstTime)
                {
                    MainDebugs = Core.DebugInformations.Where(x => x.Main && !x.Name.EndsWith("[P]") && !x.Name.EndsWith("[R]"))
                        .ToList();

                    NotMainDebugs = Core.DebugInformations.Where(x => !x.Main).ToList();
                    PluginsDebug = Core.DebugInformations.Where(x => x.Name.EndsWith("[P]") || x.Name.EndsWith("[R]")).ToList();
                    firstTime = false;
                }
                else
                {
                    foreach (DebugInformation x in args.NewItems)
                    {
                        if (x.Main && !x.Name.EndsWith("[P]") && !x.Name.EndsWith("[R]"))
                            MainDebugs.Add(x);
                        else if (x.Name.EndsWith("[P]") || x.Name.EndsWith("[R]"))
                            PluginsDebug.Add(x);
                        else
                            NotMainDebugs.Add(x);
                    }
                }

                /*foreach (DebugInformation argsNewItem in args.NewItems)
                {

                DebugWindow.LogMsg($"{argsNewItem.Name}",15,Color.Aqua);
                }*/
            }
        }

        public unsafe void Render(GameController _gameController)
        {
            if (plugins == null)
                plugins = core.pluginManager.Plugins.OrderBy(x => x.Name).ToList();

            if (_CoreSettings.ShowDemoWindow)
            {
                demo_window = true;
                ImGui.ShowDemoWindow(ref demo_window);
                _CoreSettings.ShowDemoWindow.Value = demo_window;
            }

            if (_CoreSettings.ShowDebugWindow) debugInformation.TickAction(DebugWindowRender);

            if (_CoreSettings.MainMenuKeyToggle.PressedOnce())
            {
                _CoreSettings.Enable.Value = !_CoreSettings.Enable;

                if (_CoreSettings.Enable)
                {
                    core.Graphics.LowLevel.ImGuiRender.TransparentState = false;
                    plugins = core.pluginManager.Plugins.OrderBy(x => x.Name).ToList();
                }
                else
                {
                    _settingsContainer.SaveCoreSettings();

                    if (_gameController != null)
                    {
                        foreach (var plugin in core.pluginManager.Plugins)
                        {
                            _settingsContainer.SaveSettings(plugin.Plugin);
                        }
                    }

                    core.Graphics.LowLevel.ImGuiRender.TransparentState = true;
                }
            }


            IsOpened = _CoreSettings.Enable;
            if (!_CoreSettings.Enable) return;

            ImGui.PushFont(core.Graphics.Font.Atlas);
            ImGui.SetNextWindowSize(new Vector2(800, 600), ImGuiCond.FirstUseEver);
            var pOpen = _CoreSettings.Enable.Value;
            ImGui.Begin("HUD S3ttings", ref pOpen);
            _CoreSettings.Enable.Value = pOpen;

            ImGui.BeginChild("Left menu window", new Vector2(PluginNameWidth, ImGui.GetContentRegionAvail().Y), true,
                ImGuiWindowFlags.None);

            if (ImGui.Selectable("Core", _index == -1))
            {
                _index = -1;
                Selected = CoreSettings;
            }

            ImGui.Separator();

            if (ImGui.Selectable("ThemeEditor", _index == -2))
            {
                _index = -2;
                Selected = () => { themeEditor.DrawSettingsMenu(); };
            }

            if (_gameController != null && core.pluginManager != null)
            {
                for (var index = 0; index < plugins.Count; index++)
                {
                    var plugin = plugins[index];
                    var temp = plugin.IsEnable;
                    if (ImGui.Checkbox($"##{plugin.Name}{index}", ref temp)) plugin.TurnOnOffPlugin(temp);
                    ImGui.SameLine();

                    if (ImGui.Selectable(plugin.Name, _index == index))
                    {
                        _index = index;
                        Selected = () => plugin.DrawSettings();
                    }
                }
            }

            ImGui.EndChild();
            ImGui.SameLine();
            ImGui.BeginChild("Options", ImGui.GetContentRegionAvail(), true);
            Selected?.Invoke();
            ImGui.EndChild();
            ImGui.End();
            ImGui.PopFont();
        }

        private void DebugWindowRender()
        {
            // MoreInformation?.Invoke();
            var debOpen = _CoreSettings.ShowDebugWindow.Value;
            ImGui.Begin("Debug window", ref debOpen);
            _CoreSettings.ShowDebugWindow.Value = debOpen;

            if (sw.ElapsedMilliseconds > 1000)
            {
                sw.Restart();
                refresh = true;
            }

            //Tabs before 1.67
/*            for (var index = 0; index < ((Windows[]) WindowsName).Length; index++)
            {
                var s = ((Windows[]) WindowsName)[index];
                if(index>0)
                { ImGui.SameLine();}

                if (!Equals(OpenWindow, s))
                {
                    if (ImGui.Button($"{s}##WindowName"))
                    {
                        OpenWindow = s;
                    }
                }
                else
                {
                    ImGui.TextColored(Color.OrangeRed.ToImguiV4(),$"{s}");
                }
            }*/

            ImGui.Text("Program work: ");
            ImGui.SameLine();
            ImGui.TextColored(Color.GreenYellow.ToImguiVec4(), swStartedProgram.ElapsedMilliseconds.ToString());
            ImGui.BeginTabBar("Performance tabs");

            for (var index = 0; index < ((Windows[]) WindowsName).Length; index++)
            {
                var s = ((Windows[]) WindowsName)[index];

                if (ImGui.BeginTabItem($"{s}##WindowName"))
                {
                    OpenWindow = s;
                    ImGui.EndTabItem();
                }
            }

            ImGui.EndTabBar();

            switch (OpenWindow)
            {
                case Windows.MainDebugs:
                {
                    ImGui.Columns(6, "Deb", true);
                    ImGui.SetColumnWidth(0, 200);
                    ImGui.SetColumnWidth(1, 75);
                    ImGui.SetColumnWidth(2, 75);
                    ImGui.SetColumnWidth(3, 100);
                    ImGui.SetColumnWidth(4, 100);
                    ImGui.Text("Name");
                    ImGui.NextColumn();
                    ImGui.TextUnformatted("%");
                    ImGui.NextColumn();
                    ImGui.Text("Tick");
                    ImGui.NextColumn();
                    ImGui.Text("Total");
                    ImGui.SameLine();
                    ImGui.TextDisabled("(?)");

                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
                    {
                        ImGui.SetTooltip(
                            $"Update every {DebugInformation.SizeArray / _CoreSettings.TargetFps} sec. Time to next update: {(DebugInformation.SizeArray - Core.DebugInformations[0].Index) / (float) _CoreSettings.TargetFps:0.00} sec.");
                    }

                    ImGui.NextColumn();
                    ImGui.Text("Total %%");
                    ImGui.NextColumn();
                    ImGui.Text($"Data for {DebugInformation.SizeArray / _CoreSettings.TargetFps} sec.");
                    ImGui.NextColumn();

                    foreach (var deb in MainDebugs)
                    {
                        DrawInfoForDebugInformation(deb, Core.DebugInformations[0], MainDebugs.Count);
                    }

                    ImGui.Columns(1, "", false);
                    break;
                }

                case Windows.NotMainDebugs:
                {
                    ImGui.Columns(4, "Deb", true);
                    ImGui.SetColumnWidth(0, 200);
                    ImGui.SetColumnWidth(1, 75);
                    ImGui.SetColumnWidth(2, 75);
                    ImGui.Text("Name");
                    ImGui.NextColumn();
                    ImGui.Text("Tick");
                    ImGui.NextColumn();
                    ImGui.Text("Total");
                    ImGui.SameLine();
                    ImGui.TextDisabled("(?)");

                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
                    {
                        ImGui.SetTooltip(
                            $"Update every {DebugInformation.SizeArray / _CoreSettings.TargetFps} sec. Time to next update: {(DebugInformation.SizeArray - Core.DebugInformations[0].Index) / (float) _CoreSettings.TargetFps:0.00} sec.");
                    }

                    ImGui.NextColumn();

                    ImGui.Text($"Data for {DebugInformation.SizeArray / _CoreSettings.TargetFps} sec.");
                    ImGui.NextColumn();

                    foreach (var deb in NotMainDebugs)
                    {
                        DrawInfoForNotMainDebugInformation(deb);
                    }

                    ImGui.Columns(1, "", false);
                    break;
                }

                case Windows.Plugins when AllPlugins == null:
                    AllPlugins = Core.DebugInformations.FirstOrDefault(x => x.Name == "All plugins");
                    break;
                case Windows.Plugins:
                {
                    ImGui.Columns(6, "Deb", true);
                    ImGui.SetColumnWidth(0, 200);
                    ImGui.SetColumnWidth(1, 75);
                    ImGui.SetColumnWidth(2, 75);
                    ImGui.SetColumnWidth(3, 100);
                    ImGui.SetColumnWidth(4, 100);
                    ImGui.Text("Name");
                    ImGui.NextColumn();
                    ImGui.TextUnformatted("%");
                    ImGui.NextColumn();
                    ImGui.Text("Tick");
                    ImGui.NextColumn();
                    ImGui.Text("Total");
                    ImGui.SameLine();
                    ImGui.TextDisabled("(?)");

                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
                    {
                        ImGui.SetTooltip(
                            $"Update every {DebugInformation.SizeArray / _CoreSettings.TargetFps} sec. Time to next update: {(DebugInformation.SizeArray - AllPlugins.Index) / (float) _CoreSettings.TargetFps:0.00} sec.");
                    }

                    ImGui.NextColumn();
                    ImGui.Text("Total %%");
                    ImGui.NextColumn();
                    ImGui.Text($"Data for {DebugInformation.SizeArray / _CoreSettings.TargetFps} sec.");
                    ImGui.NextColumn();
                    DrawInfoForDebugInformation(AllPlugins, Core.DebugInformations[0], MainDebugs.Count);

                    foreach (var deb in PluginsDebug)
                    {
                        DrawInfoForDebugInformation(deb, AllPlugins, PluginsDebug.Count);
                    }

                    ImGui.Columns(1, "", false);
                    break;
                }

                case Windows.Coroutines:
                    DrawCoroutineRunnerInfo(core.CoroutineRunner);
                    DrawCoroutineRunnerInfo(core.CoroutineRunnerParallel);

                    if (ImGui.CollapsingHeader("Finished coroutines"))
                    {
                        foreach (var runner in Core.MainRunner.FinishedCoroutines)
                        {
                            ImGui.Text($"{runner.Name} - {runner.Ticks} - {runner.OwnerName} - {runner.Started} - {runner.Finished}");
                        }

                        foreach (var runner in Core.ParallelRunner.FinishedCoroutines)
                        {
                            ImGui.Text($"{runner.Name} - {runner.Ticks} - {runner.OwnerName} - {runner.Started} - {runner.Finished}");
                        }
                    }

                    break;
                case Windows.Caches:
                    ImGui.Columns(6, "Cache table", true);

                    ImGui.Text("Name");
                    ImGui.NextColumn();
                    ImGui.Text("Count");
                    ImGui.NextColumn();
                    ImGui.Text("Memory read");
                    ImGui.NextColumn();
                    ImGui.Text("Cache read");
                    ImGui.NextColumn();
                    ImGui.Text("Deleted");
                    ImGui.NextColumn();
                    ImGui.Text("%% Read from memory");
                    ImGui.NextColumn();

                    var cache = core.GameController.Cache;

                    //Elements
                    ImGui.Text("Elements");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticCacheElements.Count}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticCacheElements.ReadMemory}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticCacheElements.ReadCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticCacheElements.DeletedCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticCacheElements.Coeff} %%");
                    ImGui.NextColumn();

                    //Components
                    ImGui.Text("Components");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticCacheComponents.Count}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticCacheComponents.ReadMemory}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticCacheComponents.ReadCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticCacheComponents.DeletedCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticCacheComponents.Coeff} %%");
                    ImGui.NextColumn();

                    //Entity
                    ImGui.Text("Entity");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticEntityCache.Count}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticEntityCache.ReadMemory}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticEntityCache.ReadCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticEntityCache.DeletedCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticEntityCache.Coeff} %%");
                    ImGui.NextColumn();

                    //Entity list parse
                    ImGui.Text("Entity list parse");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticEntityListCache.Count}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticEntityListCache.ReadMemory}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticEntityListCache.ReadCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticEntityListCache.DeletedCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticEntityListCache.Coeff} %%");
                    ImGui.NextColumn();

                    //Server entity
                    ImGui.Text("Server Entity list parse");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticServerEntityCache.Count}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticServerEntityCache.ReadMemory}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticServerEntityCache.ReadCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticServerEntityCache.DeletedCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StaticServerEntityCache.Coeff}%%");
                    ImGui.NextColumn();

                    //Read strings
                    ImGui.Text("String cache");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StringCache.Count}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StringCache.ReadMemory}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StringCache.ReadCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StringCache.DeletedCache}");
                    ImGui.NextColumn();
                    ImGui.Text($"{cache.StringCache.Coeff} %%");
                    ImGui.NextColumn();
                    ImGui.Columns(1, "", false);
                    break;
            }

            MoreInformation?.Invoke();
            ImGui.End();
            refresh = false;
        }

        private void DrawInfoForNotMainDebugInformation(DebugInformation deb)
        {
            if (selectedName == deb.Name) ImGui.PushStyleColor(ImGuiCol.Text, Color.OrangeRed.ToImgui());
            ImGui.Text($"{deb.Name}");

            if (ImGui.IsItemClicked() && deb.Index > 0)
                MoreInformation = () => { AddtionalInfo(deb); };

            if (!string.IsNullOrEmpty(deb.Description))
            {
                ImGui.SameLine();
                ImGui.TextDisabled("(?)");
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.None)) ImGui.SetTooltip(deb.Description);
            }

            ImGui.NextColumn();
            ImGui.Text($"{deb.Tick:0.0000}");
            ImGui.NextColumn();
            ImGui.TextUnformatted($"{deb.Total:0.000}");
            ImGui.NextColumn();
            float averageF = 0;
            float min = 0;

            if (deb.AtLeastOneFullTick)
            {
                averageF = deb.Ticks.AverageF();
                min = deb.Ticks.MinF();
            }
            else
            {
                var enumerable = deb.Ticks.Take(deb.Index).ToArray();

                if (enumerable.Length > 0)
                {
                    averageF = enumerable.Average();
                    min = enumerable.Min();
                }
            }

            ImGui.Text($"Min: {min:0.000} Max: {deb.Ticks.MaxF():00.000} Avg: {averageF:0.000} TAMax: {deb.TotalMaxAverage:00.000}");

            if (averageF >= _CoreSettings.LimitDrawPlot)
            {
                ImGui.SameLine();
                ImGui.PlotLines($"##Plot{deb.Name}", ref deb.Ticks[0], DebugInformation.SizeArray);
            }

            ImGui.Separator();
            ImGui.NextColumn();

            if (selectedName == deb.Name) ImGui.PopStyleColor();
        }

        private void AddtionalInfo(DebugInformation deb)
        {
            // ImGui.SetNextWindowPos(new Vector2(50,50));
            //  ImGui.SetNextWindowSize(new Vector2(800,300));
            //   ImGui.Begin($"{deb.Name}##moreinf",ImGuiWindowFlags.NoCollapse|ImGuiWindowFlags.NoSavedSettings);
            selectedName = deb.Name;

            if (!deb.AtLeastOneFullTick)
            {
                ImGui.Text(
                    $"Info {deb.Name} - {DebugInformation.SizeArray / _CoreSettings.TargetFps / 60f:0.00} sec. Index: {deb.Index}/{DebugInformation.SizeArray}");

                var scaleMin = deb.Ticks.Min();
                var scaleMax = deb.Ticks.Max();
                var windowWidth = ImGui.GetWindowWidth();

                ImGui.PlotHistogram($"##Plot{deb.Name}", ref deb.Ticks[0], DebugInformation.SizeArray, 0,
                    $"Avg: {deb.Ticks.Where(x => x > 0).Average():0.0000} Max {scaleMax:0.0000}", scaleMin, scaleMax,
                    new Vector2(windowWidth - 10, 150));

                if (ImGui.Button($"Close##{deb.Name}")) MoreInformation = null;
            }
            else
            {
                ImGui.Text(
                    $"Info {deb.Name} - {DebugInformation.SizeArray * DebugInformation.SizeArray / _CoreSettings.TargetFps / 60f:0.00} sec. Index: {deb.Index}/{DebugInformation.SizeArray}");

                var scaleMin = deb.TicksAverage.MinF();
                var scaleMax = deb.TicksAverage.MaxF();
                var scaleTickMax = deb.Ticks.MaxF();
                var windowWidth = ImGui.GetWindowWidth();

                ImGui.PlotHistogram($"##Plot{deb.Name}", ref deb.Ticks[0], DebugInformation.SizeArray, 0, $"{deb.Tick:0.000}", 0,
                    scaleTickMax, new Vector2(windowWidth - 50, 150));

                var enumerable = deb.TicksAverage.Where(x => x > 0).ToArray();

                if (enumerable.Length > 0)
                {
                    ImGui.Text($"Index: {deb.IndexTickAverage}/{DebugInformation.SizeArray}");

                    ImGui.PlotHistogram($"##Plot{deb.Name}", ref deb.TicksAverage[0], DebugInformation.SizeArray, 0,
                        $"Avg: {enumerable.Average():0.0000} Max {scaleMax:0.0000}", scaleMin, scaleMax,
                        new Vector2(windowWidth - 50, 150));
                }
                else
                    ImGui.Text("Dont have information");

                if (ImGui.Button($"Close##{deb.Name}")) MoreInformation = null;
            }

            // ImGui.End();
        }

        private void DrawInfoForDebugInformation(DebugInformation deb, DebugInformation total, int groupCount)
        {
            if (selectedName == deb.Name) ImGui.PushStyleColor(ImGuiCol.Text, Color.OrangeRed.ToImgui());
            ImGui.Text($"{deb.Name}");

            if (ImGui.IsItemClicked())
                MoreInformation = () => { AddtionalInfo(deb); };

            if (!string.IsNullOrEmpty(deb.Description))
            {
                ImGui.SameLine();
                ImGui.TextDisabled("(?)");
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.None)) ImGui.SetTooltip(deb.Description);
            }

            ImGui.NextColumn();
            var averageF = deb.Ticks.AverageF();
            var avgFromAllPlugins = total.Average / groupCount;
            var byName = Color.Yellow.ToImguiVec4();

            if (averageF <= avgFromAllPlugins * 0.5f)
                byName = Color.Green.ToImguiVec4();
            else if (averageF >= avgFromAllPlugins * 4f)
                byName = Color.Red.ToImguiVec4();
            else if (averageF >= avgFromAllPlugins * 1.5f) byName = Color.Orange.ToImguiVec4();

            ImGui.TextColored(byName, $"{deb.Sum / total.Sum * 100:0.00} %%");
            ImGui.NextColumn();
            ImGui.TextColored(byName, $"{deb.Tick:0.0000}");
            ImGui.NextColumn();
            ImGui.TextColored(byName, $"{deb.Total:0.000}");
            ImGui.NextColumn();
            ImGui.TextColored(byName, $"{deb.Total / total.Total * 100:0.00} %%");
            ImGui.NextColumn();

            ImGui.Text(
                $"Min: {deb.Ticks.Min():0.000} Max: {deb.Ticks.MaxF():00.000} Avg: {averageF:0.000} TAMax: {deb.TotalMaxAverage:00.000}");

            if (averageF >= _CoreSettings.LimitDrawPlot)
            {
                ImGui.SameLine();
                ImGui.PlotLines($"##Plot{deb.Name}", ref deb.Ticks[0], DebugInformation.SizeArray);
            }

            ImGui.Separator();
            ImGui.NextColumn();
            if (selectedName == deb.Name) ImGui.PopStyleColor();
        }

        private void DrawCoroutineRunnerInfo(Runner runner)
        {
            ImGui.Separator();
            ImGui.Text($"{runner.Name}");
            ImGui.Columns(11, "CoroutineTable", true);
            ImGui.Text("Name");
            ImGui.NextColumn();
            ImGui.Text("Owner");
            ImGui.NextColumn();
            ImGui.Text("Ticks");
            ImGui.NextColumn();
            ImGui.Text("Time ms");
            ImGui.NextColumn();
            ImGui.Text("Started");
            ImGui.NextColumn();
            ImGui.Text("Timeout");
            ImGui.SameLine();
            ImGui.NextColumn();
            ImGui.Text("DoWork");
            ImGui.NextColumn();
            ImGui.Text("AutoResume");
            ImGui.NextColumn();
            ImGui.Text("Done");
            ImGui.NextColumn();
            ImGui.Text("Priority");
            ImGui.NextColumn();
            ImGui.Text("DO");
            ImGui.NextColumn();
            var coroutines = runner.Coroutines.OrderByDescending(x => x.Priority).ToList();

            for (var i = 0; i < coroutines.Count(); i++)
            {
                var type = "";
                if (coroutines[i].Condition != null) type = coroutines[i].Condition.GetType().Name;
                ImGui.Text($"{coroutines[i].Name}");
                ImGui.NextColumn();
                ImGui.Text($"{coroutines[i].OwnerName}");
                ImGui.NextColumn();
                ImGui.Text($"{coroutines[i].Ticks}");
                ImGui.NextColumn();
                runner.CoroutinePerformance.TryGetValue(coroutines[i].Name, out var time);
                ImGui.Text($"{Math.Round(time, 2)}");
                ImGui.NextColumn();
                ImGui.Text($"{coroutines[i].Started.ToLongTimeString()}");
                ImGui.NextColumn();
                ImGui.Text($"{type}: {coroutines[i].TimeoutForAction}");
                ImGui.NextColumn();
                ImGui.Text($"{coroutines[i].Running}");
                ImGui.NextColumn();
                ImGui.Text($"{coroutines[i].AutoResume}");
                ImGui.NextColumn();
                ImGui.Text($"{coroutines[i].IsDone}");
                ImGui.NextColumn();
                ImGui.Text($"{coroutines[i].Priority}");
                ImGui.NextColumn();

                if (coroutines[i].Running)
                {
                    if (ImGui.Button($"Pause##{coroutines[i].Name}##{runner.Name}")) coroutines[i].Pause();
                }
                else
                {
                    if (ImGui.Button($"Start##{coroutines[i].Name}##{runner.Name}")) coroutines[i].Resume();
                }

                ImGui.SameLine();
                if (ImGui.Button($"Done##{coroutines[i].Name}##{runner.Name}")) coroutines[i].Done();
                ImGui.NextColumn();
            }

            ImGui.Columns(1, "", false);
        }

        private enum Windows
        {
            MainDebugs,
            NotMainDebugs,
            Plugins,
            Coroutines,
            Caches
        }
    }
}
