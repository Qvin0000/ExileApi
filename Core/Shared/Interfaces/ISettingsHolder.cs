using System;
using System.Collections.Generic;

namespace ExileCore.Shared.Interfaces
{
    public interface ISettingsHolder
    {
        string Name { get; set; }
        string Tooltip { get; set; }
        string Unique { get; }
        int ID { get; set; }
        Action DrawDelegate { get; set; }
        IList<ISettingsHolder> Children { get; }
        void Draw();
    }
}
