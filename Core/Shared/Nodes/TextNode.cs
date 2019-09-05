using System;
using Exile;
using Shared.Static;
using Newtonsoft.Json;

namespace Shared.Nodes
{
    public class TextNode
    {
        [JsonIgnore] public Action OnValueChanged = delegate { };
        private string value;

        public TextNode() { }

        public TextNode(string value) => Value = value;

        public void SetValueNoEvent(string newValue) => value = newValue;


        public string Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;

                    try
                    {
                        OnValueChanged();
                    }
                    catch (Exception)
                    {
                        DebugWindow.LogMsg("Error in function that subscribed for: TextNode.OnValueChanged", 10, SharpDX.Color.Red);
                    }
                }
            }
        }

        public static implicit operator string(TextNode node) => node.Value;

        public static implicit operator TextNode(string value) => new TextNode(value);
    }
}