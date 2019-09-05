using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Enums;
using SharpDX;

namespace Exile
{
    public class PluginPanel
    {
        private readonly Direction direction;

        public bool Used => settings.Any(x => x.Invoke());

        private List<Func<bool>> settings = new List<Func<bool>>();
        public void WantUse(Func<bool> enabled) => settings.Add(enabled);

        public PluginPanel(Vector2 startDrawPoint, Direction direction = Direction.Down) : this(direction) =>
            StartDrawPoint = startDrawPoint;

        public PluginPanel(Direction direction = Direction.Down) {
            this.direction = direction;
            Margin = new Vector2(0, 0);
        }


        public Vector2 StartDrawPoint { get; set; }

        public Vector2 Margin { get; }
    }
}