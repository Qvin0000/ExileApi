using System;

namespace Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MenuAttribute : Attribute
    {
        public string MenuName = "";
        public string Tooltip;
        public int index = -1;
        public int parentIndex = -1;

        public MenuAttribute(string menuName) => MenuName = menuName;

        public MenuAttribute(string menuName, string tooltip) : this(menuName) => Tooltip = tooltip;

        public MenuAttribute(string menuName, int index) {
            MenuName = menuName;
            this.index = index;
        }

        public MenuAttribute(string menuName, string tooltip, int index) : this(menuName, index) => Tooltip = tooltip;

        public MenuAttribute(string menuName, int index, int parentIndex) {
            MenuName = menuName;
            this.index = index;
            this.parentIndex = parentIndex;
        }

        public MenuAttribute(string menuName, string tooltip, int index, int parentIndex) : this(menuName, index, parentIndex) =>
            Tooltip = tooltip;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreMenuAttribute : Attribute
    {
    }
}