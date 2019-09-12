namespace ExileCore.PoEMemory.MemoryObjects
{
    public class ItemMod : RemoteMemoryObject
    {
        private string displayName;
        private string group;
        private int level;
        private string name;
        private string rawName;
        public int Value1 => M.Read<int>(Address, 0);
        public int Value2 => M.Read<int>(Address, 4);
        public int Value3 => M.Read<int>(Address, 8);
        public int Value4 => M.Read<int>(Address, 0xC);

        public string RawName
        {
            get
            {
                if (rawName == null)
                    ParseName();

                return rawName;
            }
        }

        public string Group
        {
            get
            {
                if (group == null)
                    ParseName();

                return group;
            }
        }

        public string Name
        {
            get
            {
                if (rawName == null)
                    ParseName();

                return name;
            }
        }

        public string DisplayName
        {
            get
            {
                if (rawName == null)
                    ParseName();

                return displayName;
            }
        }

        public int Level
        {
            get
            {
                if (rawName == null)
                    ParseName();

                return level;
            }
        }

        private void ParseName()
        {
            var addr = M.Read<long>(Address + 0x20, 0);
            rawName = Cache.StringCache.Read($"{nameof(ItemMod)}{addr}", () => M.ReadStringU(addr));

            displayName = Cache.StringCache.Read($"{nameof(ItemMod)}{addr + (rawName.Length + 2) * 2}",
                () => M.ReadStringU(addr + (rawName.Length + 2) * 2));

            name = rawName.Replace("_", ""); // Master Crafted mod can have underscore on the end, need to ignore
            group = Cache.StringCache.Read($"{nameof(ItemMod)}{Address + 0x20}", () => M.ReadStringU(M.Read<long>(Address + 0x20, 0x70)));
            var ixDigits = name.IndexOfAny("0123456789".ToCharArray());

            if (ixDigits < 0 || !int.TryParse(name.Substring(ixDigits), out level))
                level = 1;
            else
                name = name.Substring(0, ixDigits);
        }

        public override string ToString()
        {
            return $"{Name} ({Value1}, {Value2}, {Value3}, {Value4}";
        }
    }
}
