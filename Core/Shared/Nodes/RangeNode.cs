using System;
using Exile;
using Shared.Static;
using Newtonsoft.Json;

namespace Shared.Nodes
{
    public sealed class RangeNode<T> where T : struct
    {
        private T _value;
        public RangeNode() { }

        public RangeNode(T value, T min, T max) {
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
                        DebugWindow.LogMsg("Error in function that subscribed for: RangeNode.OnValueChanged", 10, SharpDX.Color.Red);
                    }
                }
            }
        }

        public event EventHandler<T> OnValueChanged;

        [JsonIgnore]
        public T Min { get; set; }

        [JsonIgnore]
        public T Max { get; set; }

        public static implicit operator T(RangeNode<T> node) => node.Value;
    }
}