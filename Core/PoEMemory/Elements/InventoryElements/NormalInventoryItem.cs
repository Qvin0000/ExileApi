using System;
using Exile.PoEMemory.MemoryObjects;
using Shared.Enums;
using Shared.Enums;
using GameOffsets;
using Shared.Helpers;
using Shared.Interfaces;

namespace PoEMemory.InventoryElements
{
    public class NormalInventoryItem : Element
    {
        private static int InventPosXOff = Extensions.GetOffset<NormalInventoryItemOffsets>(nameof(NormalInventoryItemOffsets.InventPosX));
        private static int InventPosYOff = Extensions.GetOffset<NormalInventoryItemOffsets>(nameof(NormalInventoryItemOffsets.InventPosY));
        private static int WidthOff = Extensions.GetOffset<NormalInventoryItemOffsets>(nameof(NormalInventoryItemOffsets.Width));
        private static int HeightOff = Extensions.GetOffset<NormalInventoryItemOffsets>(nameof(NormalInventoryItemOffsets.Height));
        private static int ItemOff = Extensions.GetOffset<NormalInventoryItemOffsets>(nameof(NormalInventoryItemOffsets.Item));
        public virtual int InventPosX => cachedValue.Value.InventPosX; //M.Read<int>(Address + InventPosXOff);
        public virtual int InventPosY => cachedValue.Value.InventPosY; //M.Read<int>(Address + InventPosYOff);
        public virtual int ItemWidth => cachedValue.Value.Width;       //M.Read<int>(Address + WidthOff);
        public virtual int ItemHeight => cachedValue.Value.Height;     //M.Read<int>(Address + HeightOff);
        private Entity _item;

        public Entity Item
        {
            get
            {
                if (_item == null) _item = GetObject<Entity>(cachedValue.Value.Item);
                return _item;
            }
        }

        public ToolTipType toolTipType => ToolTipType.InventoryItem;

        private Lazy<NormalInventoryItemOffsets> cachedValue;

        public NormalInventoryItem() =>
            cachedValue = new Lazy<NormalInventoryItemOffsets>(() => M.Read<NormalInventoryItemOffsets>(Address));

        //public Element ToolTip => ReadObject<Element>(Address + 0xB20);

        //0xB40 0xB48 some inf about image DDS
    }
}