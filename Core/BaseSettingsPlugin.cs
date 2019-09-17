using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.AtlasHelper;
using ExileCore.Shared.Interfaces;
using Newtonsoft.Json;
using SharpDX;

namespace ExileCore
{
    public abstract class BaseSettingsPlugin<TSettings> : IPlugin where TSettings : ISettings, new()
    {
        private TSettings settings;
        private AtlasTexturesProcessor _atlasTextures;

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

        public virtual void EntityAdded(Entity Entity)
        {
        }

        public virtual void EntityAddedAny(Entity Entity)
        {
        }

        public virtual void EntityIgnored(Entity Entity)
        {
        }

        public virtual void EntityRemoved(Entity Entity)
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

        public void SetApi(object gameController, object graphics)
        {
            SetApi((GameController) gameController, (Graphics) graphics);
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

        private void SetApi(GameController gameController, Graphics graphics)
        {
            GameController = gameController;
            Graphics = graphics;
        }

        #region Atlas Images

        public AtlasTexture GetAtlasTexture(string textureName)
        {
            var atlasTexturePath = Path.Combine(DirectoryFullName, "textures\\atlas.png");

            if (!File.Exists(atlasTexturePath))
            {
                LogError($"Plugin '{Name}': Can't find atlas texture file in '{atlasTexturePath}'", 20);
                return null;
            }

            var atlasConfigPath = Path.Combine(DirectoryFullName, "textures\\atlas.json");

            if (!File.Exists(atlasConfigPath))
            {
                LogError($"Plugin '{Name}': Can't find atlas json config file in '{atlasConfigPath}' (expecting config 'from Free texture packer' program)", 20);
                return null;
            }

            if (_atlasTextures == null)
            {
                _atlasTextures = new AtlasTexturesProcessor(atlasConfigPath, atlasTexturePath);
                Graphics.InitImage(atlasTexturePath, false);
            }

            var texture = _atlasTextures.GetTextureByName(textureName);

            if (texture == null)
            {
                LogError($"Plugin '{Name}': Texture with name'{textureName}' is not found in texture atlas.", 20);
            }

            return texture;
        }

        #endregion
    }
}
