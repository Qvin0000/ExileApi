using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Nodes;
using Newtonsoft.Json;
using Shared.Nodes;

namespace Exile.RenderQ
{
    public class ThemeEditor
    {
        private readonly CoreSettings coreSettings;
        public const string ThemeExtension = ".hudtheme";
        public const string DefaultThemeName = "Default";
        private const string ThemesFolder = "config/themes";

        private ThemeConfig LoadedTheme;

        public ThemeEditor(CoreSettings coreSettings) {
            this.coreSettings = coreSettings;

            GenerateDefaultTheme();

            if (!Directory.Exists(ThemesFolder))
            {
                Directory.CreateDirectory(ThemesFolder);
                var defaultTheme = GenerateDefaultTheme();
                SaveTheme(defaultTheme, DefaultThemeName);
                coreSettings.Theme.Value = DefaultThemeName;
            }

            LoadThemeFilesList();
            SelectedThemeName = coreSettings.Theme.Value ?? coreSettings.Theme.Values.FirstOrDefault();
            ApplyTheme(SelectedThemeName);
            coreSettings.Theme.OnValueSelected += ApplyTheme;
        }

        private void LoadThemeFilesList() {
            var fi = new DirectoryInfo(ThemesFolder);
            coreSettings.Theme.Values = fi.GetFiles($"*{ThemeExtension}").OrderByDescending(x => x.LastWriteTime)
                                          .Select(x => Path.GetFileNameWithoutExtension(x.Name)).ToList();
        }


        private string SelectedThemeName;
        private int SelectedThemeId;
        private string NewThemeName = "MyNewTheme";

        public void DrawSettingsMenu() {
            if (ImGui.Combo("Select Theme", ref SelectedThemeId, coreSettings.Theme.Values.ToArray(), coreSettings.Theme.Values.Count))
            {
                if (SelectedThemeName != coreSettings.Theme.Values[SelectedThemeId])
                {
                    SelectedThemeName = coreSettings.Theme.Values[SelectedThemeId];
                    LoadedTheme = LoadTheme(coreSettings.Theme.Values[SelectedThemeId], false);
                    ApplyTheme(LoadedTheme);
                }
            }

            if (ImGui.Button("Save current theme settings to selected"))
            {
                var currentThemeNew = ReadThemeFromCurrent();
                SaveTheme(currentThemeNew, SelectedThemeName);
            }

            ImGui.Text("");
            ImGui.InputText("New theme name", ref NewThemeName, 200, ImGuiInputTextFlags.None);

            if (ImGui.Button("Create new theme from current"))
            {
                if (!string.IsNullOrEmpty(NewThemeName))
                {
                    var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                    var r = new Regex($"[{Regex.Escape(regexSearch)}]");
                    NewThemeName = r.Replace(NewThemeName, "");

                    var currentThemeNew = ReadThemeFromCurrent();
                    SaveTheme(currentThemeNew, NewThemeName);
                    SelectedThemeName = NewThemeName;
                    LoadThemeFilesList();
                }
            }

            ImGui.Text("");

            var style = ImGui.GetStyle();

            if (ImGui.TreeNode("Theme settings"))
            {
                style.AntiAliasedFill = DrawBoolSetting("AntiAliasedFill", style.AntiAliasedFill);
                style.AntiAliasedLines = DrawBoolSetting("AntiAliasedLines", style.AntiAliasedLines);

                style.Alpha = DrawFloatSetting("Alpha", style.Alpha * 100, 0, 100) / 100f;

                style.DisplayWindowPadding = DrawVectorSetting("DisplayWindowPadding", style.DisplayWindowPadding, 0, 20);
                style.TouchExtraPadding = DrawVectorSetting("TouchExtraPadding", style.TouchExtraPadding, 0, 10);
                style.WindowPadding = DrawVectorSetting("WindowPadding", style.WindowPadding, 0, 20);
                style.FramePadding = DrawVectorSetting("FramePadding", style.FramePadding, 0, 20);
                style.DisplaySafeAreaPadding = DrawVectorSetting("DisplaySafeAreaPadding", style.DisplaySafeAreaPadding, 0, 20);

                style.ItemInnerSpacing = DrawVectorSetting("ItemInnerSpacing", style.ItemInnerSpacing, 0, 20);
                style.ItemSpacing = DrawVectorSetting("ItemSpacing", style.ItemSpacing, 0, 20);

                style.GrabMinSize = DrawFloatSetting("GrabMinSize", style.GrabMinSize, 0, 20);
                style.GrabRounding = DrawFloatSetting("GrabRounding", style.GrabRounding, 0, 12);
                style.IndentSpacing = DrawFloatSetting("IndentSpacing", style.IndentSpacing, 0, 30);

                style.ScrollbarRounding = DrawFloatSetting("ScrollbarRounding", style.ScrollbarRounding, 0, 19);
                style.ScrollbarSize = DrawFloatSetting("ScrollbarSize", style.ScrollbarSize, 0, 20);

                style.WindowTitleAlign = DrawVectorSetting("WindowTitleAlign", style.WindowTitleAlign, 0, 1, 0.1f);
                style.WindowRounding = DrawFloatSetting("WindowRounding", style.WindowRounding, 0, 14);
                style.ChildRounding = DrawFloatSetting("ChildWindowRounding", style.ChildRounding, 0, 16);
                style.FrameRounding = DrawFloatSetting("FrameRounding", style.FrameRounding, 0, 12);
                style.ColumnsMinSpacing = DrawFloatSetting("ColumnsMinSpacing", style.ColumnsMinSpacing, 0, 30);

                style.CurveTessellationTol = DrawFloatSetting("CurveTessellationTolerance", style.CurveTessellationTol * 100, 0, 100) / 100;
            }

            ImGui.Text("");
            ImGui.Text("Colors:");
            ImGui.Columns(2, "Columns", true);

            var colorTypes = Enum.GetValues(typeof(ImGuiCol)).Cast<ImGuiCol>();
            var count = colorTypes.Count() / 2;

            foreach (var type in colorTypes)
                unsafe
                {
                    var nameFixed = Regex.Replace(type.ToString(), "(\\B[A-Z])", " $1");

                    var styleColorVec4 = ImGui.GetStyleColorVec4(type);
                    var colorValue = new Vector4(styleColorVec4->X, styleColorVec4->Y, styleColorVec4->Z, styleColorVec4->W);


                    if (ImGui.ColorEdit4(nameFixed, ref colorValue,
                                         ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.NoInputs |
                                         ImGuiColorEditFlags.AlphaPreviewHalf))
                        //    style.SetColor(type, colorValue);
                        ImGui.PushStyleColor(type, colorValue);
                    if (count-- == -1)
                        ImGui.NextColumn();
                }
        }

        private bool DrawBoolSetting(string name, bool result) {
            ImGui.Checkbox(name, ref result);
            return result;
        }

        private float DrawFloatSetting(string name, float result, float min, float max, float power = 1) {
            ImGui.SliderFloat(name, ref result, min, max, "%.0f", power);
            return result;
        }

        private Vector2 DrawVectorSetting(string name, Vector2 result, float min, float max, float power = 1) {
            ImGui.SliderFloat2(name, ref result, min, max, "%.0f", power);
            return result;
        }

        public static void ApplyTheme(string fileName) {
            var theme = LoadTheme(fileName, true);

            if (theme == null)
            {
                DebugWindow.LogMsg($"Can't find theme file {fileName}, loading default.", 3);
                theme = LoadTheme(DefaultThemeName, true);
                if (theme == null)
                {
                    DebugWindow.LogMsg($"Can't find default theme file {DefaultThemeName}, Generating default and saving...", 3);
                    theme = GenerateDefaultTheme();
                    SaveTheme(theme, DefaultThemeName);
                }
            }

            ApplyTheme(theme);
        }

        public static void ApplyTheme(ThemeConfig theme) {
            var style = ImGui.GetStyle();

            style.AntiAliasedLines = theme.AntiAliasedLines;
            style.DisplaySafeAreaPadding = theme.DisplaySafeAreaPadding;
            style.DisplayWindowPadding = theme.DisplayWindowPadding;
            style.GrabRounding = theme.GrabRounding;
            style.GrabMinSize = theme.GrabMinSize;
            style.ScrollbarRounding = theme.ScrollbarRounding;
            style.ScrollbarSize = theme.ScrollbarSize;
            style.ColumnsMinSpacing = theme.ColumnsMinSpacing;
            style.IndentSpacing = theme.IndentSpacing;
            style.TouchExtraPadding = theme.TouchExtraPadding;
            style.ItemInnerSpacing = theme.ItemInnerSpacing;

            style.ItemSpacing = theme.ItemSpacing;
            style.FrameRounding = theme.FrameRounding;
            style.FramePadding = theme.FramePadding;
            style.ChildRounding = theme.ChildWindowRounding;
            style.WindowTitleAlign = theme.WindowTitleAlign;
            style.WindowRounding = theme.WindowRounding;
            //style.WindowMinSize = theme.WindowMinSize;
            style.WindowPadding = theme.WindowPadding;
            style.Alpha = theme.Alpha;
            style.AntiAliasedFill = theme.AntiAliasedFill;
            style.CurveTessellationTol = theme.CurveTessellationTolerance;


            foreach (var color in theme.Colors)
                try
                {
                    if (color.Key == ImGuiCol.COUNT) //This shit made a crash
                        continue;
                    ImGui.PushStyleColor(color.Key, color.Value);
                }
                catch (Exception ex)
                {
                    DebugWindow.LogError(ex.Message, 5);
                }
        }

        private ThemeConfig ReadThemeFromCurrent() {
            var style = ImGui.GetStyle();
            var result = new ThemeConfig
            {
                AntiAliasedLines = style.AntiAliasedLines,
                DisplaySafeAreaPadding = style.DisplaySafeAreaPadding,
                DisplayWindowPadding = style.DisplayWindowPadding,
                GrabRounding = style.GrabRounding,
                GrabMinSize = style.GrabMinSize,
                ScrollbarRounding = style.ScrollbarRounding,
                ScrollbarSize = style.ScrollbarSize,
                ColumnsMinSpacing = style.ColumnsMinSpacing,
                IndentSpacing = style.IndentSpacing,
                TouchExtraPadding = style.TouchExtraPadding,
                ItemInnerSpacing = style.ItemInnerSpacing,
                ItemSpacing = style.ItemSpacing,
                FrameRounding = style.FrameRounding,
                FramePadding = style.FramePadding,
                ChildWindowRounding = style.ChildRounding,
                WindowTitleAlign = style.WindowTitleAlign,
                WindowRounding = style.WindowRounding,
                WindowPadding = style.WindowPadding,
                Alpha = style.Alpha,
                AntiAliasedFill = style.AntiAliasedFill,
                CurveTessellationTolerance = style.CurveTessellationTol
            };
            //result.WindowMinSize = style.WindowMinSize;

            var colorTypeValues = Enum.GetValues(typeof(ImGuiCol)).Cast<ImGuiCol>();
            //Read colors
            foreach (var colorType in colorTypeValues)
                unsafe
                {
                    if (colorType == ImGuiCol.COUNT) //This shit made a crash
                        continue;

                    var SP4 = ImGui.GetStyleColorVec4(colorType);
                    result.Colors.Add(colorType, new Vector4(SP4->X, SP4->Y, SP4->Z, SP4->W));
                }

            return result;
        }

        private static ThemeConfig GenerateDefaultTheme() {
            var resultTheme = new ThemeConfig();
            resultTheme.Colors.Add(ImGuiCol.Text, new Vector4(0.90f, 0.90f, 0.90f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.TextDisabled, new Vector4(0.60f, 0.60f, 0.60f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.WindowBg, new Vector4(0.16f, 0.16f, 0.16f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.ChildBg, new Vector4(0.12f, 0.12f, 0.12f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.PopupBg, new Vector4(0.11f, 0.11f, 0.14f, 0.92f));
            resultTheme.Colors.Add(ImGuiCol.Border, new Vector4(0.44f, 0.44f, 0.44f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.BorderShadow, new Vector4(0.00f, 0.00f, 0.00f, 0.00f));
            resultTheme.Colors.Add(ImGuiCol.FrameBg, new Vector4(0.20f, 0.20f, 0.20f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.FrameBgHovered, new Vector4(0.98f, 0.61f, 0.26f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.FrameBgActive, new Vector4(0.74f, 0.36f, 0.02f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.TitleBg, new Vector4(0.40f, 0.19f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.TitleBgActive, new Vector4(0.74f, 0.36f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.TitleBgCollapsed, new Vector4(0.75f, 0.37f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.MenuBarBg, new Vector4(0.29f, 0.29f, 0.30f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.ScrollbarBg, new Vector4(0.28f, 0.28f, 0.28f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.ScrollbarGrab, new Vector4(0.71f, 0.37f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.ScrollbarGrabHovered, new Vector4(0.86f, 0.41f, 0.06f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.ScrollbarGrabActive, new Vector4(0.64f, 0.29f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.CheckMark, new Vector4(0.96f, 0.45f, 0.01f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.SliderGrab, new Vector4(0.86f, 0.48f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.SliderGrabActive, new Vector4(0.52f, 0.31f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.Button, new Vector4(0.73f, 0.37f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.ButtonHovered, new Vector4(0.97f, 0.57f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.ButtonActive, new Vector4(0.62f, 0.29f, 0.01f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.Header, new Vector4(0.59f, 0.28f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.HeaderHovered, new Vector4(0.74f, 0.35f, 0.02f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.HeaderActive, new Vector4(0.88f, 0.45f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.Separator, new Vector4(0.50f, 0.50f, 0.50f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.SeparatorHovered, new Vector4(0.60f, 0.60f, 0.70f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.SeparatorActive, new Vector4(0.70f, 0.70f, 0.90f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.ResizeGrip, new Vector4(1.00f, 1.00f, 1.00f, 0.16f));
            resultTheme.Colors.Add(ImGuiCol.ResizeGripHovered, new Vector4(0.78f, 0.82f, 1.00f, 0.60f));
            resultTheme.Colors.Add(ImGuiCol.ResizeGripActive, new Vector4(0.78f, 0.82f, 1.00f, 0.90f));
            //  resultTheme.Colors.Add(ImGuiCol.CloseButton, new Vector4(0.50f, 0.50f, 0.90f, 0.50f));
            //   resultTheme.Colors.Add(ImGuiCol.CloseButtonHovered, new Vector4(0.70f, 0.70f, 0.90f, 0.60f));
            //    resultTheme.Colors.Add(ImGuiCol.CloseButtonActive, new Vector4(0.70f, 0.70f, 0.70f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.PlotLines, new Vector4(1.00f, 1.00f, 1.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.PlotLinesHovered, new Vector4(0.90f, 0.70f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.PlotHistogram, new Vector4(0.90f, 0.70f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.PlotHistogramHovered, new Vector4(1.00f, 0.60f, 0.00f, 1.00f));
            resultTheme.Colors.Add(ImGuiCol.TextSelectedBg, new Vector4(1.00f, 0.03f, 0.03f, 0.35f));
            resultTheme.Colors.Add(ImGuiCol.ModalWindowDimBg, new Vector4(0.20f, 0.20f, 0.20f, 0.35f));
            resultTheme.Colors.Add(ImGuiCol.DragDropTarget, new Vector4(1.00f, 1.00f, 0.00f, 0.90f));
            return resultTheme;
        }

        #region SaveLoad

        private static ThemeConfig LoadTheme(string fileName, bool nullIfNotFound) {
            ThemeConfig result;
            try
            {
                var fullPath = Path.Combine(ThemesFolder, fileName + ThemeExtension);
                if (File.Exists(fullPath))
                {
                    var json = File.ReadAllText(fullPath);
                    return JsonConvert.DeserializeObject<ThemeConfig>(json, SettingsContainer.jsonSettings);
                }
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Error while loading theme {fileName}: {ex.Message}, Generating default one", 3);
            }

            if (nullIfNotFound)
                return null;
            return GenerateDefaultTheme();
        }

        private static void SaveTheme(ThemeConfig theme, string fileName) {
            try
            {
                var fullPath = Path.Combine(ThemesFolder, fileName + ThemeExtension);
                var settingsDirName = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(settingsDirName))
                    Directory.CreateDirectory(settingsDirName);

                using (var stream = new StreamWriter(File.Create(fullPath)))
                {
                    var json = JsonConvert.SerializeObject(theme, Formatting.Indented, SettingsContainer.jsonSettings);
                    stream.Write(json);
                }
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"Error while loading theme: {ex.Message}", 3);
            }
        }

        #endregion
    }

    public class ThemeConfig : ISettings
    {
        public ThemeConfig() => Enable = new ToggleNode(true);
        public ToggleNode Enable { get; set; }


        #region ThemeSettings

        //
        // Summary:
        //     Enable anti-aliasing on lines/borders. Disable if you are really tight on CPU/GPU.
        public bool AntiAliasedLines { get; set; } = true;

        //
        // Summary:
        //     If you cannot see the edge of your screen (e.g. on a TV) increase the safe area
        //     padding. Covers popups/tooltips as well regular windows.
        public Vector2 DisplaySafeAreaPadding { get; set; } = Vector2.One * 8;

        //
        // Summary:
        //     Window positions are clamped to be visible within the display area by at least
        //     this amount. Only covers regular windows.
        public Vector2 DisplayWindowPadding { get; set; } = Vector2.One * 8;

        //
        // Summary:
        //     Radius of grabs corners rounding. Set to 0.0f to have rectangular slider grabs.
        public float GrabRounding { get; set; } = 0;

        //
        // Summary:
        //     Minimum width/height of a grab box for slider/scrollbar
        public float GrabMinSize { get; set; } = 10;

        //
        // Summary:
        //     Radius of grab corners for scrollbar
        public float ScrollbarRounding { get; set; } = 9;

        //
        // Summary:
        //     Width of the vertical scrollbar, Height of the horizontal scrollbar
        public float ScrollbarSize { get; set; } = 16;

        //
        // Summary:
        //     Minimum horizontal spacing between two columns
        public float ColumnsMinSpacing { get; set; } = 21;

        //
        // Summary:
        //     Horizontal indentation when e.g. entering a tree node
        public float IndentSpacing { get; set; } = 21;

        //
        // Summary:
        //     Expand reactive bounding box for touch-based system where touch position is not
        //     accurate enough. Unfortunately we don't sort widgets so priority on overlap will
        //     always be given to the first widget. So don't grow this too much!
        public Vector2 TouchExtraPadding { get; set; } = Vector2.Zero;

        //
        // Summary:
        //     Horizontal and vertical spacing between within elements of a composed widget
        //     (e.g. a slider and its label).
        public Vector2 ItemInnerSpacing { get; set; } = Vector2.One * 4;

        //
        // Summary:
        //     Horizontal and vertical spacing between widgets/lines.
        public Vector2 ItemSpacing { get; set; } = new Vector2(8, 4);

        //
        // Summary:
        //     Radius of frame corners rounding. Set to 0.0f to have rectangular frame (used
        //     by most widgets).
        public float FrameRounding { get; set; } = 0;

        //
        // Summary:
        //     Padding within a framed rectangle (used by most widgets).
        public Vector2 FramePadding { get; set; } = new Vector2(4, 3);

        //
        // Summary:
        //     Radius of child window corners rounding. Set to 0.0f to have rectangular windows.
        public float ChildWindowRounding { get; set; } = 0;

        //
        // Summary:
        //     Alignment for title bar text.
        public Vector2 WindowTitleAlign { get; set; } = Vector2.One * 0.5f;

        //
        // Summary:
        //     Radius of window corners rounding. Set to 0.0f to have rectangular windows.
        public float WindowRounding { get; set; } = 7;

        //
        // Summary:
        //     Minimum window size.
        //public Vector2 WindowMinSize { get; set; }
        //
        // Summary:
        //     Padding within a window.
        public Vector2 WindowPadding { get; set; } = Vector2.One * 8;

        //
        // Summary:
        //     Global alpha applies to everything in ImGui.
        public float Alpha { get; set; } = 1f;

        //
        // Summary:
        //     Enable anti-aliasing on filled shapes (rounded rectangles, circles, etc.)
        public bool AntiAliasedFill { get; set; } = true;

        //
        // Summary:
        //     Tessellation tolerance. Decrease for highly tessellated curves (higher quality,
        //     more polygons), increase to reduce quality.
        public float CurveTessellationTolerance { get; set; } = 1f;

        #endregion


        public Dictionary<ImGuiCol, Vector4> Colors = new Dictionary<ImGuiCol, Vector4>();
    }
}