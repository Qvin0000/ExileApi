using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Nodes;
using Shared.Static;
using ImGuiNET;
using JM.LinqFaster;
using MoreLinq;
using Shared.Attributes;
using Shared.Nodes;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.WIC;
using Vector2 = System.Numerics.Vector2;

namespace Exile
{
    public static class SettingsParser
    {
        public static void Parse(ISettings settings, List<ISettingsHolder> draws, int id = -1) {
            if (settings == null)
            {
                DebugWindow.LogError($"Cant parse null settings.");
                return;
            }

            var props = settings.GetType().GetProperties();

            foreach (var property in props)
            {
                if (property.GetCustomAttribute<IgnoreMenuAttribute>() != null) continue;
                var menuAttribute = property.GetCustomAttribute<MenuAttribute>();
                var isSettings = property.PropertyType.GetInterfaces().ContainsF(typeof(ISettings));

                if (property.Name == "Enable" && menuAttribute == null) continue;
                if (menuAttribute == null)
                    menuAttribute = new MenuAttribute(System.Text.RegularExpressions.Regex.Replace(property.Name, "(\\B[A-Z])", " $1"));
                var holder = new SettingsHolder
                {
                    Name = menuAttribute.MenuName,
                    Tooltip = menuAttribute.Tooltip,
                    ID = menuAttribute.index == -1 ? MathHepler.Randomizer.Next(int.MaxValue) : menuAttribute.index
                };

                if (isSettings)
                {
                    var innerSettings = (ISettings) property.GetValue(settings);
                    if (menuAttribute.index != -1)
                    {
                        holder.Type = HolderChildType.Tab;
                        draws.Add(holder);
                        Parse(innerSettings, draws, menuAttribute.index);
                        var parent = GetAllDrawers(draws).Find(x => x.ID == menuAttribute.parentIndex);
                        parent?.Children.Add(holder);
                    }
                    else
                        Parse(innerSettings, draws);

                    continue;
                }


                var type = property.GetValue(settings);


                if (menuAttribute.parentIndex != -1)
                {
                    var parent = GetAllDrawers(draws).Find(x => x.ID == menuAttribute.parentIndex);
                    parent?.Children.Add(holder);
                }
                else if (id != -1)
                {
                    var parent = GetAllDrawers(draws).Find(x => x.ID == id);
                    parent?.Children.Add(holder);
                }
                else
                    draws.Add(holder);

                switch (type)
                {
                    case ButtonNode n:
                        holder.DrawDelegate = () =>
                        {
                            if (ImGui.Button(holder.Unique)) n.OnPressed();
                        };
                        break;
                    case EmptyNode n:

                        break;
                    case HotkeyNode n:
                        holder.DrawDelegate = () =>
                        {
                            var holderName = $"{holder.Name} {n.Value}##{n.Value}";
                            var open = true;
                            if (ImGui.Button(holderName))
                            {
                                ImGui.OpenPopup(holderName);
                                open = true;
                            }

                            if (ImGui.BeginPopupModal(holderName, ref open, (ImGuiWindowFlags) 35))
                            {
                                if (Input.GetKeyState(Keys.Escape))
                                {
                                    ImGui.CloseCurrentPopup();
                                    ImGui.EndPopup();
                                    return;
                                }
                                else
                                {
                                    foreach (var key in Enum.GetValues(typeof(Keys)))
                                    {
                                        var keyState = Input.GetKeyState((Keys) key);
                                        if (keyState)
                                        {
                                            n.Value = (Keys) key;
                                            ImGui.CloseCurrentPopup();
                                            break;
                                        }
                                    }
                                }

                                ImGui.Text($" Press new key to change '{n.Value}' or Esc for exit.");

                                ImGui.EndPopup();
                            }
                        };
                        break;
                    case ToggleNode n:
                        holder.DrawDelegate = () =>
                        {
                            var value = n.Value;
                            ImGui.Checkbox(holder.Unique, ref value);
                            n.Value = value;
                        };
                        break;
                    case ColorNode n:
                        holder.DrawDelegate = () =>
                        {
                            var vector4 = n.Value.ToVector4().ToVector4Num();
                            if (ImGui.ColorEdit4(holder.Unique, ref vector4,
                                                 ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.NoInputs |
                                                 ImGuiColorEditFlags.AlphaPreviewHalf)) n.Value = vector4.ToSharpColor();
                        };
                        break;
                    case ListNode n:
                        holder.DrawDelegate = () =>
                        {
                            if (ImGui.BeginCombo(holder.Unique, n.Value))
                            {
                                foreach (var t in n.Values)
                                    if (ImGui.Selectable(t))
                                    {
                                        n.Value = t;
                                        ImGui.EndCombo();
                                        return;
                                    }

                                ImGui.EndCombo();
                            }
                        };
                        break;
                    case FileNode n:
                        holder.DrawDelegate = () =>
                        {
                            if (ImGui.TreeNode(holder.Unique))
                            {
                                var selected = n.Value;
                                if (ImGui.BeginChildFrame(1, new Vector2(0, 300)))
                                {
                                    var di = new DirectoryInfo("config");
                                    if (di.Exists)
                                    {
                                        foreach (var file in di.GetFiles())
                                            if (ImGui.Selectable(file.Name, selected == file.FullName))
                                                n.Value = file.FullName;
                                    }

                                    ImGui.EndChildFrame();
                                }

                                ImGui.TreePop();
                            }
                        };
                        break;
                    case RangeNode<int> n:
                        holder.DrawDelegate = () =>
                        {
                            var r = n.Value;
                            ImGui.SliderInt(holder.Unique, ref r, n.Min, n.Max);
                            n.Value = r;
                        };
                        break;
                    case RangeNode<float> n:

                        holder.DrawDelegate = () =>
                        {
                            var r = n.Value;
                            ImGui.SliderFloat(holder.Unique, ref r, n.Min, n.Max);
                            n.Value = r;
                        };
                        break;
                    case RangeNode<long> n:
                        holder.DrawDelegate = () =>
                        {
                            var r = (int) n.Value;
                            ImGui.SliderInt(holder.Unique, ref r, (int) n.Min, (int) n.Max);
                            n.Value = r;
                        };
                        break;
                    case RangeNode<Vector2> n:
                        holder.DrawDelegate = () =>
                        {
                            var vect = n.Value;
                            ImGui.SliderFloat2(holder.Unique, ref vect, n.Min.X, n.Max.X);
                            n.Value = vect;
                        };
                        break;
                    default:
                        Core.Logger.Warning($"{type} not supported for menu now. Ask developers to add this type.");
                        break;
                }
            }
        }

        private static List<ISettingsHolder> GetAllDrawers(List<ISettingsHolder> SettingPropertyDrawers) {
            var result = new List<ISettingsHolder>();
            GetDrawersRecurs(SettingPropertyDrawers, result);
            return result;
        }

        private static void GetDrawersRecurs(IList<ISettingsHolder> drawers, IList<ISettingsHolder> result) {
            foreach (var drawer in drawers)
                if (!result.Contains(drawer))
                    result.Add(drawer);
                else
                    Core.Logger.Error(
                        $" Possible stashoverflow or duplicating drawers detected while generating menu. Drawer SettingName: {drawer.Name}, Id: {drawer.ID}",
                        5);

            drawers.ForEach(x => GetDrawersRecurs(x.Children, result));
        }
    }

    public enum HolderChildType
    {
        Tab,
        Border
    }

    public class SettingsHolder : ISettingsHolder
    {
        public string Name { get; set; } = "";
        public string Tooltip { get; set; }
        public string Unique => $"{Name}##{ID}";
        public int ID { get; set; } = -1;
        public Action DrawDelegate { get; set; }
        public IList<ISettingsHolder> Children { get; } = new List<ISettingsHolder>();
        public HolderChildType Type { get; set; } = HolderChildType.Border;

        public SettingsHolder() => Tooltip = "";

        public void Draw() {
            var size = ImGui.GetFont();


            if (Children.Count > 0)
            {
                for (var i = 0; i < 5; i++) ImGui.Spacing();


                ImGui.BeginGroup();
                var contentRegionAvail = ImGui.GetContentRegionAvail();

                var OverChild = ImGui.GetCursorPos().Translate(10, size.FontSize * -0.66f);

                ImGui.BeginChild(Unique, new Vector2(contentRegionAvail.X, size.FontSize * 2 * (Children.Count + 0.2f)), true);

                foreach (var child in Children) child.Draw();
                // var fontContainer = Fonts.Last().Value;
                // ImGui.PushFont(fontContainer.Atlas);

                var getCursor = ImGui.GetCursorPos().Translate(0, size.FontSize);
                ImGui.EndChild();
                ImGui.SetCursorPos(OverChild);
                ImGui.Text(Name);
                if (Tooltip?.Length > 0)
                {
                    ImGui.SameLine();
                    ImGui.TextDisabled("(?)");
                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.None)) ImGui.SetTooltip(Tooltip);
                }

                ImGui.SetCursorPos(getCursor);
                ImGui.EndGroup();

                DrawDelegate?.Invoke();


                //  ImGui.PopFont();
            }
            else
            {
                DrawDelegate?.Invoke();
                if (Tooltip?.Length > 0)
                {
                    ImGui.SameLine();
                    ImGui.TextDisabled("(?)");
                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.None)) ImGui.SetTooltip(Tooltip);
                }
            }
        }
    }
}