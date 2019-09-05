using System.Collections.Generic;
using System.Linq;
using Exile;
using Shared.Static;
using Shared.Enums;

namespace PoEMemory
{
    public class MapSubInventoryInfo
    {
        public int Tier { get; set; }
        public int Count { get; set; }
        public string MapName { get; set; }
        public override string ToString() => $"Tier:{Tier} Count:{Count} MapName:{MapName}";
    }


    public class MapSubInventoryKey
    {
        public string Path { get; set; }
        public MapType Type { get; set; }
        public override string ToString() => $"Path:{Path} Type:{Type}";
    }


    public class MapStashTabElement : Element
    {
        private long mapListStartPtr => Address != 0 ? M.Read<long>(Address + 0x9D8) : 0x00;
        private long mapListEndPtr => Address != 0 ? M.Read<long>(Address + 0x9D8 + 0x08) : 0x00;
        public int TotalInventories => (int) ((mapListEndPtr - mapListStartPtr) / 0x10);

        public Dictionary<MapSubInventoryKey, MapSubInventoryInfo> MapsCount => GetMapsCount();

        private Dictionary<MapSubInventoryKey, MapSubInventoryInfo> GetMapsCount() {
            var result = new Dictionary<MapSubInventoryKey, MapSubInventoryInfo>();
            MapSubInventoryInfo subInventoryInfo = null;
            MapSubInventoryKey subInventoryKey = null;

            var totalInventories = TotalInventories;
            if (totalInventories > 1024)
            {
                DebugWindow.LogError(
                    $"{nameof(MapStashTabElement)}-> {nameof(GetMapsCount)} error {nameof(TotalInventories)} = {totalInventories}");
                return null;
            }

            for (var i = 0; i < totalInventories; i++)
            {
                subInventoryInfo = new MapSubInventoryInfo();
                subInventoryKey = new MapSubInventoryKey();
                subInventoryInfo.Tier = SubInventoryMapTier(i);
                subInventoryInfo.Count = SubInventoryMapCount(i);
                subInventoryInfo.MapName = SubInventoryMapName(i);
                /*if (subInventoryInfo.Count == 0)
                    continue;*/
                subInventoryKey.Path = SubInventoryMapPath(i);
                subInventoryKey.Type = SubInventoryMapType(i);
                result.Add(subInventoryKey, subInventoryInfo);
            }

            return result;
        }

        public Dictionary<string, string> MapsCountByName => GetMapsCount2();

        private Dictionary<string, string> GetMapsCount2() {
            var maps = GetMapsCount();
            var result = new Dictionary<string, string>();

            foreach (var mapSubInventoryInfo in maps.OrderBy(x => x.Value.Tier))
            {
                var shaped = mapSubInventoryInfo.Key.Type == MapType.Shaped ? "Shaped" : "";
                var name = $"{mapSubInventoryInfo.Value.Tier}: {shaped} {mapSubInventoryInfo.Value.MapName}";
                var info = $"{mapSubInventoryInfo.Value.Count}";
                result[name] = info;
            }

            return result;
        }

        private int SubInventoryMapTier(int index) => M.Read<int>(mapListStartPtr + index * 0x10, 0x00);

        private int SubInventoryMapCount(int index) => M.Read<int>(mapListStartPtr + index * 0x10, 0x08);

        private MapType SubInventoryMapType(int index) => (MapType) M.Read<int>(mapListStartPtr + index * 0x10, 0x10);

        private string SubInventoryMapPath(int index) => M.ReadStringU(M.Read<long>(mapListStartPtr + index * 0x10, 0x28, 0x00));

        private string SubInventoryMapName(int index) => M.ReadStringU(M.Read<long>(mapListStartPtr + index * 0x10, 0x28, 0x20));
        public Dictionary<string, string> MapsCountByTier => GetMapsCountFromUi();
        public Dictionary<string, string> CurrentCell => GetCurrentCell();
        public Dictionary<string, Element> CurrentCellElements => GetCurrentCellElements();

        public Dictionary<string, Element> TierElements => GetTierElements();

        private Dictionary<string, Element> GetCurrentCellElements() {
            var cell = Children[2].Children[0].Children[0].Children;
            var result = new Dictionary<string, Element>();
            foreach (var element in cell)
            {
                var name = element?.Tooltip?.Children?[0].Children[0].Children[3].Text;
                if (name == null)
                {
                    var tooltipText = element.Tooltip?.Text;
                    if (tooltipText != null)
                        name = tooltipText.Substring(0, tooltipText.IndexOf('\n'));
                    else
                        name = "Error";
                }


                var count = element.Children[4].Text;
                result.Add(name, element);
            }

            return result;
        }

        private Dictionary<string, string> GetCurrentCell() {
            var cell = Children[2].Children[0].Children[0].Children;
            var result = new Dictionary<string, string>();
            foreach (var element in cell)
            {
                var name = element?.Tooltip?.Children?[0].Children[0].Children[3].Text;
                if (name == null)
                {
                    var tooltipText = element.Tooltip?.Text;
                    if (tooltipText != null)
                        name = tooltipText.Substring(0, tooltipText.IndexOf('\n'));
                    else
                        name = "Error";
                }


                var count = element.Children[4].Text;
                result.Add(name, count);
            }

            return result;
        }


        private Dictionary<string, string> GetMapsCountFromUi() {
            var Rows = Children[0].Children.Concat(Children[1].Children);
            var result = new Dictionary<string, string>();
            foreach (var element in Rows) result.Add(element.Children[0].Text, element.Children[1].Text);

            return result;
        }

        private Dictionary<string, Element> GetTierElements() {
            var Rows = Children[0].Children.Concat(Children[1].Children);
            var result = new Dictionary<string, Element>();
            var i = 1;
            foreach (var element in Rows)
            {
                var text = element.Children[0].Text;
                if (text != "U")
                {
                    text = i.ToString();
                    i++;
                }

                result[text] = element;
            }

            return result;
        }
    }
}