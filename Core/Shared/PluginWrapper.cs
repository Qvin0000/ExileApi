using System;
using System.Diagnostics;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.Shared
{
    public class PluginWrapper
    {
        private static readonly Stopwatch sw = Stopwatch.StartNew();
        private double startTick;

        public PluginWrapper(IPlugin plugin)
        {
            Plugin = plugin;

            lock (Core.SyncLocker)
            {
                TickDebugInformation = new DebugInformation($"{Name} [P]", "plugin");
                RenderDebugInformation = new DebugInformation($"{Name} [R]", "plugin");
            }
        }

        public bool Force => Plugin.Force;
        public string Name => Plugin.Name;
        public int Order => Plugin.Order;
        public IPlugin Plugin { get; }
        public bool CanRender { get; set; }
        public bool CanBeMultiThreading => Plugin.CanUseMultiThreading;
        public DebugInformation TickDebugInformation { get; }
        public DebugInformation RenderDebugInformation { get; }
        public bool IsEnable => Plugin._Settings.Enable;

        public void CorrectThisTick(float val)
        {
            TickDebugInformation.CorrectAfterTick(val);
        }

        public void TurnOnOffPlugin(bool state)
        {
            Plugin._Settings.Enable.Value = state;
        }

        public void AreaChange(AreaInstance area)
        {
            try
            {
                Plugin.AreaChange(area);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public Job PerfomanceTick()
        {
            try
            {
                startTick = sw.Elapsed.TotalMilliseconds;
                var tick = Plugin.Tick();
                TickDebugInformation.Tick = sw.Elapsed.TotalMilliseconds - startTick;
                return tick;
            }
            catch (Exception e)
            {
                LogError(e);
                return null;
            }
        }

        public Job Tick()
        {
            try
            {
                return Plugin.Tick();
            }
            catch (Exception e)
            {
                LogError(e);
                return null;
            }
        }

        public void PerfomanceRender()
        {
            try
            {
                startTick = sw.Elapsed.TotalMilliseconds;
                Plugin.Render();
                RenderDebugInformation.Tick = sw.Elapsed.TotalMilliseconds - startTick;
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void Render()
        {
            try
            {
                Plugin.Render();
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        private void LogError(Exception e)
        {
            var msg = $"{Plugin.Name} -> {e}";
            DebugWindow.LogError(msg);
        }

        public void EntityIgnored(Entity entity)
        {
            try
            {
                Plugin.EntityIgnored(entity);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void EntityAddedAny(Entity entity)
        {
            try
            {
                Plugin.EntityAddedAny(entity);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void EntityAdded(Entity entity)
        {
            try
            {
                Plugin.EntityAdded(entity);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void EntityRemoved(Entity entity)
        {
            try
            {
                Plugin.EntityRemoved(entity);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void SetApi(GameController gameController, Graphics graphics)
        {
            Plugin.SetApi(gameController, graphics);
        }

        public void LoadSettings()
        {
            Plugin._LoadSettings();
        }

        public void Close()
        {
            try
            {
                Plugin.OnClose();
                Plugin.OnUnload();
                Plugin.Dispose();
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void DrawSettings()
        {
            Plugin.DrawSettings();
        }
    }
}
