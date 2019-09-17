using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SharpDX;

namespace ExileCore.Shared.AtlasHelper
{
    internal sealed class AtlasTexturesProcessor
    {
        private readonly Dictionary<string, AtlasTexture> _atlasTextures = new Dictionary<string, AtlasTexture>();

        public AtlasTexturesProcessor(string configPath, string atlasPath)
        {
            LoadConfig(configPath, atlasPath);
        }

        private void LoadConfig(string configPath, string atlasPath)
        {
            _atlasTextures.Clear();
            var configStr = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<AtlasConfigData>(configStr);

            var atlasSize = new Vector2(config.Meta.Size.W, config.Meta.Size.H);

            foreach (var keyValuePair in config.Frames)
            {
                var textureName = keyValuePair.Key.Replace(".png", string.Empty);

                if (string.IsNullOrEmpty(textureName))
                {
                    DebugWindow.LogError($"Sprite '{Path.GetFileNameWithoutExtension(configPath)}' contain a texture with empty/null name.", 20);
                    continue;
                }

                if (_atlasTextures.ContainsKey(textureName))
                {
                    DebugWindow.LogError(
                        $"Sprite '{Path.GetFileNameWithoutExtension(configPath)}' already have a texture with name {textureName}. Duplicates is not allowed!", 20);

                    continue;
                }

                var uvX = keyValuePair.Value.Frame.X / atlasSize.X;
                var uvY = keyValuePair.Value.Frame.Y / atlasSize.Y;
                var uvWidth = keyValuePair.Value.Frame.W / atlasSize.X;
                var uvHeight = keyValuePair.Value.Frame.H / atlasSize.Y;
                var textureUv = new RectangleF(uvX, uvY, uvWidth, uvHeight);
                var newTexture = new AtlasTexture(textureName, textureUv, atlasPath);
                _atlasTextures.Add(textureName, newTexture);
            }
        }

        public AtlasTexture GetTextureByName(string textureName)
        {
            return _atlasTextures.TryGetValue(textureName.Replace(".png", string.Empty), out var texture) ? texture : null;
        }
    }
}
