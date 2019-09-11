using System;
using Newtonsoft.Json;

namespace ExileCore.Shared.Nodes
{
    public sealed class ToggleNode
    {
        [JsonIgnore] private bool value;

        public ToggleNode()
        {
        }

        public ToggleNode(bool value)
        {
            Value = value;
        }

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

        public event EventHandler<bool> OnValueChanged;

        public void SetValueNoEvent(bool newValue)
        {
            value = newValue;
        }

        public static implicit operator bool(ToggleNode node)
        {
            return node.Value;
        }
    }
}
