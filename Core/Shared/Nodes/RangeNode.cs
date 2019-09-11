using System;
using Newtonsoft.Json;
using SharpDX;

namespace ExileCore.Shared.Nodes
{
    public sealed class RangeNode<T> where T : struct
    {
        private T _value;

        public RangeNode()
        {
        }

        public RangeNode(T value, T min, T max)
        {
            Value = value;
            Min = min;
            Max = max;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (!value.Equals(_value))
                {
                    _value = value;

                    try
                    {
                        OnValueChanged?.Invoke(this, value);
                    }
                    catch (Exception)
                    {
                        DebugWindow.LogMsg("Error in function that subscribed for: RangeNode.OnValueChanged", 10, Color.Red);
                    }
                }
            }
        }

        [JsonIgnore]
        public T Min { get; set; }
        [JsonIgnore]
        public T Max { get; set; }
        public event EventHandler<T> OnValueChanged;

        public static implicit operator T(RangeNode<T> node)
        {
            return node.Value;
        }
    }
}
