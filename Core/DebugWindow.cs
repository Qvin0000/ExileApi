using System;
using System.Collections.Generic;
using ExileCore.Shared;
using ExileCore.Shared.Helpers;
using ImGuiNET;
using SharpDX;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace ExileCore
{
    public class DebugWindow
    {
        private static readonly object locker = new object();
        private static readonly Dictionary<string, DebugMsgDescription> Messages;
        private static readonly List<DebugMsgDescription> MessagesList;
        private static readonly Queue<string> toDelete;
        private static readonly Queue<DebugMsgDescription> LogHistory;
        private static readonly CircularBuffer<DebugMsgDescription> History;
        private readonly Graphics graphics;
        private readonly CoreSettings settingsCoreSettings;
        private Vector2 position;

        static DebugWindow()
        {
            Messages = new Dictionary<string, DebugMsgDescription>(24);
            MessagesList = new List<DebugMsgDescription>(24);
            toDelete = new Queue<string>(24);
            LogHistory = new Queue<DebugMsgDescription>(1024);
            History = new CircularBuffer<DebugMsgDescription>(1024);
        }

        public DebugWindow(Graphics graphics, CoreSettings settingsCoreSettings)
        {
            this.graphics = graphics;
            this.settingsCoreSettings = settingsCoreSettings;
            graphics.InitImage("menu-background.png");
        }

        public void Render()
        {
            try
            {
                if (settingsCoreSettings.ShowDebugLog)
                {
                    unsafe
                    {
                        ImGui.PushFont(graphics.Font.Atlas);
                    }

                    ImGui.SetNextWindowPos(new Vector2(10, 10), ImGuiCond.Appearing);
                    ImGui.SetNextWindowSize(new Vector2(600, 1000), ImGuiCond.Appearing);
                    ImGui.Begin("Debug log");

                    foreach (var msg in History)
                    {
                        if (msg == null) continue;
                        ImGui.PushStyleColor(ImGuiCol.Text, msg.ColorV4);
                        ImGui.TextUnformatted($"{msg.Time.ToLongTimeString()}: {msg.Msg}");
                        ImGui.PopStyleColor();
                    }

                    ImGui.PopFont();
                    ImGui.End();
                }

                if (MessagesList.Count == 0) return;

                position = new Vector2(10, 35);

                for (var index = 0; index < MessagesList.Count; index++)
                {
                    var message = MessagesList[index];
                    if (message == null) continue;

                    if (message.Time < DateTime.UtcNow)
                    {
                        toDelete.Enqueue(message.Msg);
                        continue;
                    }

                    var draw = message.Msg;
                    if (message.Count > 1) draw = $"({message.Count}){draw}";

                    var currentPosition = graphics.DrawText(draw, position, message.Color);

                    graphics.DrawImage("menu-background.png",
                        new RectangleF(position.X - 5, position.Y, currentPosition.X + 20, currentPosition.Y));

                    position = new Vector2(position.X, position.Y + currentPosition.Y);
                }

                while (toDelete.Count > 0)
                {
                    var delete = toDelete.Dequeue();

                    if (Messages.TryGetValue(delete, out var debugMsgDescription))
                    {
                        MessagesList.Remove(debugMsgDescription);
                        LogHistory.Enqueue(debugMsgDescription);
                        History.PushBack(debugMsgDescription);

                        if (debugMsgDescription.Color == Color.Red)
                            Core.Logger.Error($"{debugMsgDescription.Msg}");
                        else
                            Core.Logger.Information($"{debugMsgDescription.Msg}");
                    }

                    Messages.Remove(delete);

                    if (LogHistory.Count >= 1024)
                        for (var i = 0; i < 24; i++)
                        {
                            LogHistory.Dequeue();
                        }
                }
            }
            catch (Exception e)
            {
                LogError(e.ToString());
            }
        }

        public static void LogMsg(string msg)
        {
            LogMsg(msg, 1f, Color.White);
        }

        public static void LogError(string msg, float time = 2f)
        {
            LogMsg(msg, time, Color.Red);
        }

        public static void LogMsg(string msg, float time)
        {
            LogMsg(msg, time, Color.White);
        }

        public static void LogMsg(string msg, float time, Color color)
        {
            try
            {
                if (Messages.TryGetValue(msg, out var result))
                {
                    result.Time = DateTime.UtcNow.AddSeconds(time);
                    result.Color = color;
                    result.Count++;
                }
                else
                {
                    result = new DebugMsgDescription
                    {
                        Msg = msg,
                        Time = DateTime.UtcNow.AddSeconds(time),
                        ColorV4 = color.ToImguiVec4(),
                        Color = color,
                        Count = 1
                    };

                    lock (locker)
                    {
                        Messages[msg] = result;
                        MessagesList.Add(result);
                    }
                }
            }
            catch (Exception e)
            {
                Core.Logger.Error($"{nameof(DebugWindow)} -> {e}");
            }
        }
    }

    public class DebugMsgDescription
    {
        public string Msg { get; set; }
        public DateTime Time { get; set; }
        public Vector4 ColorV4 { get; set; }
        public Color Color { get; set; }
        public int Count { get; set; }
    }
}
