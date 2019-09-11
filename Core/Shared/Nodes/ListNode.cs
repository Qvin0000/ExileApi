using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharpDX;

namespace ExileCore.Shared.Nodes
{
    public class ListNode
    {
        [JsonIgnore] public Action<string> OnValueSelected = delegate { };
        [JsonIgnore] public Action<string> OnValueSelectedPre = delegate { };
        private string value;
        [JsonIgnore] public List<string> Values = new List<string>();

        public string Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    try
                    {
                        OnValueSelectedPre(value);
                    }
                    catch
                    {
                        DebugWindow.LogMsg("Error in function that subscribed for: ListNode.OnValueSelectedPre", 10, Color.Red);
                    }

                    this.value = value;

                    try
                    {
                        OnValueSelected(value);
                    }
                    catch (Exception ex)
                    {
                        DebugWindow.LogMsg($"Error in function that subscribed for: ListNode.OnValueSelected. Error: {ex.Message}", 10,
                            Color.Red);
                    }
                }
            }
        }

        public static implicit operator string(ListNode node)
        {
            return node.Value;
        }

        public void SetListValues(List<string> values)
        {
            Values = values;
        }
    }
}
