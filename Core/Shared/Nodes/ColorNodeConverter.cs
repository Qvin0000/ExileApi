using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ExileCore.Shared.Nodes
{
    public class ColorNodeConverter : CustomCreationConverter<ColorNode>
    {
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override ColorNode Create(Type objectType)
        {
            return new ColorNode();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return uint.TryParse(reader.Value.ToString(), NumberStyles.HexNumber, null, out var argb) ? new ColorNode(argb) : Create(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (ColorNode) value;
            serializer.Serialize(writer, $"{color.Value.ToAbgr():x8}");
        }
    }
}
