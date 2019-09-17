using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExileCore.Shared.AtlasHelper
{
    public class AtlasConfigData
    {
        [JsonProperty("frames")]
        public Dictionary<string, FrameValue> Frames { get; set; }
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class FrameValue
    {
        [JsonProperty("frame")]
        public SpriteSourceSizeClass Frame { get; set; }
        [JsonProperty("rotated")]
        public bool Rotated { get; set; }
        [JsonProperty("trimmed")]
        public bool Trimmed { get; set; }
        [JsonProperty("spriteSourceSize")]
        public SpriteSourceSizeClass SpriteSourceSize { get; set; }
        [JsonProperty("sourceSize")]
        public Size SourceSize { get; set; }
        [JsonProperty("pivot")]
        public Pivot Pivot { get; set; }
    }

    public class SpriteSourceSizeClass
    {
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
        [JsonProperty("w")]
        public int W { get; set; }
        [JsonProperty("h")]
        public int H { get; set; }
    }

    public class Pivot
    {
        [JsonProperty("x")]
        public float X { get; set; }
        [JsonProperty("y")]
        public float Y { get; set; }
    }

    public class Size
    {
        [JsonProperty("w")]
        public int W { get; set; }
        [JsonProperty("h")]
        public int H { get; set; }
    }

    public class Meta
    {
        [JsonProperty("app")]
        public Uri App { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        [JsonProperty("format")]
        public string Format { get; set; }
        [JsonProperty("size")]
        public Size Size { get; set; }
        [JsonProperty("scale")]
        public long Scale { get; set; }
    }
}
