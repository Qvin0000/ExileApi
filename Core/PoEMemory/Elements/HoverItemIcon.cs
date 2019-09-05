using System;
using Exile;
using Exile.PoEMemory.MemoryObjects;
using GameOffsets;
using Shared.Enums;
using Shared.Static;
using PoEMemory.Components;
using Shared.Enums;
using Shared.Helpers;

namespace PoEMemory.Elements
{
    public class HoverItemIcon : Element
    {
        public Element InventoryItemTooltip => ReadObject<Element>(Address + 0x338);
        public Element ItemInChatTooltip => ReadObject<Element>(Address + 0x1A8);
        public ItemOnGroundTooltip ToolTipOnGround => TheGame.IngameState.IngameUi.ItemOnGroundTooltip;
        private ToolTipType? toolTip;
        private static int InventPosXOff = Extensions.GetOffset<NormalInventoryItemOffsets>(nameof(NormalInventoryItemOffsets.InventPosX));
        private static int InventPosYOff = Extensions.GetOffset<NormalInventoryItemOffsets>(nameof(NormalInventoryItemOffsets.InventPosY));
        public int InventPosX => M.Read<int>(Address + InventPosXOff);
        public int InventPosY => M.Read<int>(Address + InventPosYOff);


        public ToolTipType ToolTipType
        {
            get
            {
                try
                {
                    return (ToolTipType) (toolTip ?? (toolTip = GetToolTipType()));
                }
                catch (Exception e)
                {
                    Core.Logger.Error($"{e.Message} {e.StackTrace}");
                    return ToolTipType.None;
                }
            }
        }

        public Element Tooltip
        {
            get
            {
                switch (ToolTipType)
                {
                    case ToolTipType.ItemOnGround:
                        return ToolTipOnGround.Tooltip;

                    case ToolTipType.InventoryItem:
                        return InventoryItemTooltip;
                    case ToolTipType.ItemInChat:
                        return ItemInChatTooltip.Children[1];
                }

                return null;
            }
        }

        public Element ItemFrame
        {
            get
            {
                switch (ToolTipType)
                {
                    case ToolTipType.ItemOnGround:
                        return ToolTipOnGround.ItemFrame;
                    case ToolTipType.ItemInChat:
                        return ItemInChatTooltip.Children[0];
                    default:
                        return null;
                }
            }
        }

        private static int ItemsOnGroundLabelElementOffset =
            Extensions.GetOffset<IngameUElementsOffsets>(nameof(IngameUElementsOffsets.itemsOnGroundLabelRoot));

        public Entity Item
        {
            get
            {
                switch (ToolTipType)
                {
                    case ToolTipType.ItemOnGround:
                        // This offset is same as Game.IngameState.IngameUi.ItemsOnGroundLabels offset.
                        var le = TheGame.IngameState.IngameUi.ReadObjectAt<ItemsOnGroundLabelElement>(ItemsOnGroundLabelElementOffset);
                        if (le == null)
                            return null;
                        var e = le.ItemOnHover;
                        return e?.GetComponent<WorldItem>()?.ItemEntity;
                    case ToolTipType.InventoryItem:
                        return ReadObject<Entity>(Address + 0x388);
                    case ToolTipType.ItemInChat:
                        // currently cannot find it.
                        return null;
                }

                return null;
            }
        }

        private ToolTipType GetToolTipType() {
            try
            {
                if (InventoryItemTooltip != null && InventoryItemTooltip.IsVisible) return ToolTipType.InventoryItem;
                if (ToolTipOnGround != null && ToolTipOnGround.Tooltip != null && ToolTipOnGround.TooltipUI != null &&
                    ToolTipOnGround.TooltipUI.IsVisible) return ToolTipType.ItemOnGround;
                if (ItemInChatTooltip != null && ItemInChatTooltip.IsVisible && ItemInChatTooltip.ChildCount > 1 &&
                    ItemInChatTooltip.Children[0].IsVisible && ItemInChatTooltip.Children[1].IsVisible) return ToolTipType.ItemInChat;
            }
            catch (Exception e)
            {
                Core.Logger.Error($"HoverItemIcon.cs -> {e}");
            }

            return ToolTipType.None;
        }
    }
}