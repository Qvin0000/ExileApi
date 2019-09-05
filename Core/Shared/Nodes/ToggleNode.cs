using System;
using Exile;
using Shared.Static;
using Newtonsoft.Json;
using SharpDX;

namespace Shared.Nodes
{
    public sealed class ToggleNode
    {
        public event EventHandler<bool> OnValueChanged;
        [JsonIgnore] private bool value;

        public ToggleNode() { }

        public ToggleNode(bool value) => Value = value;

        public void SetValueNoEvent(bool newValue) => value = newValue;

        public bool Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    try
                    {
                        OnValueChanged?.Invoke(this, value);
                    }
                    catch (Exception e)
                    {
                        DebugWindow.LogError($"Error in function that subscribed for: ToggleNode.OnValueChanged. {Environment.NewLine} {e}",
                                             10);
                    }
                }
            }
        }

        public static implicit operator bool(ToggleNode node) => node.Value;
     
    }
}