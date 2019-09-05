using System;
using System.Collections.Generic;

namespace Shared.Interfaces
{
    public interface ISettingsHolder
    {
        string Name { get; set; }
        string Tooltip { get; set; }
        string Unique { get; }
        int ID { get; set; }
        Action DrawDelegate { get; set; }
        void Draw();
        IList<ISettingsHolder> Children { get; }
    }
}