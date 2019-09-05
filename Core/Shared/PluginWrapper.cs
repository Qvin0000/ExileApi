using System;
using System.Diagnostics;
using Exile;
using Exile.PoEMemory.MemoryObjects;
using Shared.Interfaces;
using Shared.Static;

namespace Shared
{
    public class PluginWrapper
    {
        private static Stopwatch sw = Stopwatch.StartNew();
        private double startTick;
        public bool Force => _plugin.Force;
        public string Name => _plugin.Name;
        public int Order => _plugin.Order;
        private readonly IPlugin _plugin;
        public IPlugin Plugin => _plugin;
        public bool CanRender { get; set; }
        public bool CanBeMultiThreading => Plugin.CanUseMultiThreading;
        public DebugInformation TickDebugInformation { get; }
        public DebugInformation RenderDebugInformation { get; }

        public PluginWrapper(IPlugin plugin) {
            _plugin = plugin;
            lock (Core.SyncLocker)
            {
                TickDebugInformation = new DebugInformation($"{Name} [P]", "plugin");
                RenderDebugInformation = new DebugInformation($"{Name} [R]", "plugin");
            }
        }


        public void CorrectThisTick(float val) {
            TickDebugInformation.CorrectAfterTick(val);
        }
        public bool IsEnable => _plugin._Settings.Enable;

        public void TurnOnOffPlugin(bool state) => _plugin._Settings.Enable.Value = state;

        public void AreaChange(AreaInstance area) {
            try
            {
                _plugin.AreaChange(area);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public Job PerfomanceTick() {
            try
            {
                startTick = sw.Elapsed.TotalMilliseconds;
                var tick = _plugin.Tick();
                TickDebugInformation.Tick = sw.Elapsed.TotalMilliseconds - startTick;
                return tick;
            }
            catch (Exception e)
            {
                LogError(e);
                return null;
            }
        }

        public Job Tick() {
            try
            {
              return  _plugin.Tick();
            }
            catch (Exception e)
            {
                LogError(e);
                return null;
            }
        }

        public void PerfomanceRender() {
            try
            {
                startTick = sw.Elapsed.TotalMilliseconds;
                _plugin.Render();
                RenderDebugInformation.Tick = sw.Elapsed.TotalMilliseconds - startTick;
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void Render() {
            try
            {
                _plugin.Render();
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }


        private void LogError(Exception e) {
            var msg = $"{_plugin.Name} -> {e}";
            DebugWindow.LogError(msg);
        }

        public void EntityIgnored(Entity entity) {
            try
            {
                _plugin.EntityIgnored(entity);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void EntityAddedAny(Entity entity) {
            try
            {
                _plugin.EntityAddedAny(entity);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void EntityAdded(Entity entity) {
            try
            {
                _plugin.EntityAdded(entity);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void EntityRemoved(Entity entity) {
            try
            {
                _plugin.EntityRemoved(entity);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void SetApi(GameController gameController, Graphics graphics) => _plugin.SetApi(gameController, graphics);

        public void LoadSettings() => _plugin._LoadSettings();


        public void Close() {
            try
            {
                _plugin.OnClose();
                _plugin.OnUnload();
                _plugin.Dispose();
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void DrawSettings() => _plugin.DrawSettings();
    }
}