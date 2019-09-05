using System;
using Newtonsoft.Json;

namespace Shared.Nodes
{
    public class ButtonNode
    {
        [JsonIgnore] public Action OnPressed = delegate { };

        public ButtonNode() { }
    }
}