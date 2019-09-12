using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Enums;
using SharpDX;

namespace ExileCore
{
    public class PluginPanel
    {
        private readonly Direction direction;
        private readonly List<Func<bool>> settings = new List<Func<bool>>();

        public PluginPanel(Vector2 startDrawPoint, Direction direction = Direction.Down) : this(direction)
        {
            StartDrawPoint = startDrawPoint;
        }

        public PluginPanel(Direction direction = Direction.Down)
        {
            this.direction = direction;
            Margin = new Vector2(0, 0);
        }

        public bool Used => settings.Any(x => x.Invoke());
        public Vector2 StartDrawPoint { get; set; }
        public Vector2 Margin { get; }

        public void WantUse(Func<bool> enabled)
        {
            settings.Add(enabled);
        }
    }
}
