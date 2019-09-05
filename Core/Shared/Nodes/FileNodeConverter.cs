using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shared.Nodes;

namespace Shared.Nodes
{
    public class FileNodeConverter : CustomCreationConverter<FileNode>
    {
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override FileNode Create(Type objectType) => string.Empty;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            new FileNode(serializer.Deserialize<string>(reader));

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (value is FileNode fileNode) serializer.Serialize(writer, fileNode.Value);
        }
    }
}