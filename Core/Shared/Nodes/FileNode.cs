using System;

namespace Shared.Nodes
{
    public sealed class FileNode
    {
        public FileNode() { }

        public FileNode(string value) => Value = value;

        //[JsonIgnore]
        public event EventHandler<string> OnFileChanged;

        private string value;


        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                OnFileChanged?.Invoke(this, value);
            }
        }

        public static implicit operator string(FileNode node) => node.Value;

        public static implicit operator FileNode(string value) => new FileNode(value);
    }
}