using System;

namespace ExileCore.Shared.Nodes
{
    public sealed class FileNode
    {
        private string value;

        public FileNode()
        {
        }

        public FileNode(string value)
        {
            Value = value;
        }

        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                OnFileChanged?.Invoke(this, value);
            }
        }

        //[JsonIgnore]
        public event EventHandler<string> OnFileChanged;

        public static implicit operator string(FileNode node)
        {
            return node.Value;
        }

        public static implicit operator FileNode(string value)
        {
            return new FileNode(value);
        }
    }
}
