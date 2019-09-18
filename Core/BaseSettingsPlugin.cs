using System;
using System.Collections.Generic;
using System.IO;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.AtlasHelper;
using ExileCore.Shared.Interfaces;
using Newtonsoft.Json;
using SharpDX;

namespace ExileCore
{
    public abstract class BaseSettingsPlugin<TSettings> : IPlugin where TSettings : ISettings, new()
    {                
        const string TEXTURES_FOLDER = "textures";
        private AtlasTexturesProcessor _atlasTextures;
        private PluginManager _pluginManager;

        protected BaseSettingsPlugin()
        {
            InternalName = GetType().Namespace;
            if (string.IsNullOrWhiteSpace(Name)) Name = InternalName;
            Drawers = new List<ISettingsHolder>();
        }

        public List<ISettingsHolder> Drawers { get; }
        public GameController GameController { get; private set; }
        public Graphics Graphics { get; private set; }
        public TSettings Settings => (TSettings) _Settings;
        public ISettings _Settings { get; private set; }
        public bool CanUseMultiThreading { get; protected set; }
        public string Description { get; protected set; }
        public string DirectoryName { get; set; }
        public string DirectoryFullName { get; set; }
        public bool Force { get; protected set; }
        public bool Initialized { get; set; }
        public string InternalName { get; }
        public string Name { get; set; }
        public int Order { get; protected set; }

        public void _LoadSettings()
        {
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

        public void _SaveSettings()
        {
            if (_Settings == null)
                throw new NullReferenceException("Plugin settings is null");

            GameController.Settings.SaveSettings(this);
        }

        public virtual void AreaChange(AreaInstance area)
        {
        }

        public virtual void Dispose()
        {
            OnClose();
        }

        public virtual void DrawSettings()
        {
            foreach (var drawer in Drawers)
            {
                drawer.Draw();
            }
        }

        public virtual void EntityAdded(Entity entity)
        {
        }

        public virtual void EntityAddedAny(Entity entity)
        {
        }

        public virtual void EntityIgnored(Entity entity)
        {
        }

        public virtual void EntityRemoved(Entity entity)
        {
        }

        public virtual void OnLoad()
        {
        }

        public virtual void OnUnload()
        {
        }

        public virtual bool Initialise()
        {
            return true;
        }

        public void LogMsg(string msg)
        {
            DebugWindow.LogMsg(msg);
        }

        public virtual void OnClose()
        {
            _SaveSettings();
        }

        public virtual void ReceiveEvent(string eventId, object args)
        {
        }

        public void PublishEvent(string eventId, object args)
        {
            _pluginManager.ReceivePluginEvent(eventId, args, this);
        }

        public virtual void OnPluginSelectedInMenu()
        {
        }

        public virtual Job Tick()
        {
            return null;
        }

        public virtual void Render()
        {
        }

        public void LogError(string msg, float time = 1f)
        {
            DebugWindow.LogError(msg, time);
        }

        public void LogMessage(string msg, float time, Color clr)
        {
            DebugWindow.LogMsg(msg, time, clr);
        }

        public void LogMessage(string msg, float time = 1f)
        {
            DebugWindow.LogMsg(msg, time, Color.GreenYellow);
        }

        public virtual void OnPluginDestroyForHotReload()
        {
        }

        public void SetApi(GameController gameController, Graphics graphics, PluginManager pluginManager)
        {
            GameController = gameController;
            Graphics = graphics;
            _pluginManager = pluginManager;
        }

        #region Atlas Images

        public AtlasTexture GetAtlasTexture(string textureName)
        {
            if (_atlasTextures == null)
            {
                var atlasDirectory = Path.Combine(DirectoryFullName, TEXTURES_FOLDER);
                var atlasConfigNames = Directory.GetFiles(atlasDirectory, "*.json");

                if (atlasConfigNames.Length == 0)
                {
                    LogError($"Plugin '{Name}': Can't find atlas json config file in '{atlasDirectory}' " +
                             "(expecting config 'from Free texture packer' program)", 20);

                    _atlasTextures = new AtlasTexturesProcessor("%AtlasNotFound%");
                    return null;
                }

                var atlasName = Path.GetFileNameWithoutExtension(atlasConfigNames[0]);

                if (atlasConfigNames.Length > 1)
                {
                    LogError($"Plugin '{Name}': Found multiple atlas configs in folder '{atlasDirectory}', " +
                             $"selecting the first one ''{atlasName}''", 20);
                }

                var atlasTexturePath = Path.Combine(DirectoryFullName, $"{TEXTURES_FOLDER}\\{atlasName}.png");

                if (!File.Exists(atlasTexturePath))
                {
                    LogError($"Plugin '{Name}': Can't find atlas png texture file in '{atlasTexturePath}' ", 20);
                    _atlasTextures = new AtlasTexturesProcessor(atlasName);
                    return null;
                }

                _atlasTextures = new AtlasTexturesProcessor(configPath: atlasConfigNames[0], atlasPath: atlasTexturePath);
                Graphics.InitImage(atlasTexturePath, false);
            }

            var texture = _atlasTextures.GetTextureByName(textureName);

            return texture;
        }

        #endregion
    }
}
