using System;
using Newtonsoft.Json;

namespace ExileCore.Shared.Nodes
{
    public class ButtonNode
    {
        [JsonIgnore] public Action OnPressed = delegate { };
    }
}
