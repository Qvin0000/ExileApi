using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Exile.PoEMemory.MemoryObjects;
using Shared;
using Shared.Interfaces;
using Shared.Static;
using Newtonsoft.Json;
using SharpDX;

namespace Exile
{
    public abstract class BaseSettingsPlugin<TSettings> : IPlugin where TSettings : ISettings, new()
    {
        private TSettings settings;

        public BaseSettingsPlugin() {
            InternalName = GetType().Namespace;
            if (string.IsNullOrWhiteSpace(Name)) Name = InternalName;
            Drawers = new List<ISettingsHolder>();
        }

        public ISettings _Settings { get; private set; }
        public bool CanUseMultiThreading { get; protected set; }
        public string Description { get; protected set; }
        public string DirectoryName { get; set; }
        public string DirectoryFullName { get; set; }
        public List<ISettingsHolder> Drawers { get; }
        public bool Force { get; protected set; }
        public GameController GameController { get; private set; }
        public Graphics Graphics { get; private set; }
        public bool Initialized { get; set; }
        public string InternalName { get; private set; }

        public string Name { get; set; }

        public int Order { get; protected set; }


        public TSettings Settings => (TSettings) _Settings;

        public void _LoadSettings() {
            var loadedFile = GameController.Settings.LoadSettings(this);


            if (loadedFile == null)
            {
                _Settings = new TSettings();
                _SaveSettings();
            }
            else
                _Settings = JsonConvert.DeserializeObject<TSettings>(loadedFile, SettingsContainer.jsonSettings);

            SettingsParser.Parse(_Settings, Drawers);
        }

        public void _SaveSettings() {
            if (_Settings == null)
                throw new NullReferenceException("Plugin settings is null");
            GameController.Settings.SaveSettings(this);
        }

        public virtual void AreaChange(AreaInstance area) { }

        public virtual void Dispose() => OnClose();

        public virtual void DrawSettings() {
            foreach (var drawer in Drawers) drawer.Draw();
        }

        public virtual void EntityAdded(Entity Entity) { }
        public virtual void EntityAddedAny(Entity Entity) { }

        public virtual void EntityIgnored(Entity Entity) { }

        public virtual void EntityRemoved(Entity Entity) { }
        public virtual void OnLoad() { }
        public virtual void OnUnload() { }

        public virtual bool Initialise() => false;

        public void LogError(string msg, float time = 1f) => DebugWindow.LogError(msg, time);

        public void LogMessage(string msg, float time, Color clr) => DebugWindow.LogMsg(msg, time, clr);

        public void LogMessage(string msg, float time = 1f) => DebugWindow.LogMsg(msg, time, Color.GreenYellow);

        public void LogMsg(string msg) => DebugWindow.LogMsg(msg);

        public virtual void OnClose() => _SaveSettings();

        public virtual void OnPluginDestroyForHotReload() { }

        public virtual void OnPluginSelectedInMenu() { }
        public virtual Job Tick() => null;

        public virtual void Render() { }

        public void SetApi(object gameController, object graphics) => SetApi((GameController) gameController, (Graphics) graphics);

        private void SetApi(GameController gameController, Graphics graphics) {
            GameController = gameController;
            Graphics = graphics;
        }
    }
}