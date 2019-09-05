using System;
using System.Collections.Generic;
using Exile;
using Shared.Static;
using Newtonsoft.Json;

namespace Shared.Nodes
{
    public class ListNode
    {
        [JsonIgnore] public Action<string> OnValueSelected = delegate { };
        [JsonIgnore] public Action<string> OnValueSelectedPre = delegate { };

        private string value;

        public ListNode() { }


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
                        DebugWindow.LogMsg("Error in function that subscribed for: ListNode.OnValueSelectedPre", 10, SharpDX.Color.Red);
                    }

                    this.value = value;

                    try
                    {
                        OnValueSelected(value);
                    }
                    catch (Exception ex)
                    {
                        DebugWindow.LogMsg($"Error in function that subscribed for: ListNode.OnValueSelected. Error: {ex.Message}", 10,
                                               SharpDX.Color.Red);
                    }
                }
            }
        }

        public static implicit operator string(ListNode node) => node.Value;

        [JsonIgnore] public List<string> Values = new List<string>();

        public void SetListValues(List<string> values) => Values = values;
    }
}