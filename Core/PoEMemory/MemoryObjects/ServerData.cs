using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Exile.PoEMemory.MemoryObjects;
using Shared.Helpers;
using GameOffsets;
using PoEMemory.Components;
using PoEMemory.MemoryObjects;
using Shared.Interfaces;
using Shared.Enums;

namespace PoEMemory
{
    public class ServerData : RemoteMemoryObject
    {
        private CachedValue<ServerDataOffsets> _cachedValue;

        public ServerDataOffsets ServerDataStruct => _cachedValue.Value;

        public ServerData() =>
            _cachedValue = new FrameCache<ServerDataOffsets>(() => M.Read<ServerDataOffsets>(Address + ServerDataOffsets.Skip));

        public ushort TradeChatChannel => ServerDataStruct.TradeChatChannel;
        public ushort GlobalChatChannel => ServerDataStruct.GlobalChatChannel;
        public byte MonsterLevel => ServerDataStruct.MonsterLevel;

        //if 51 - more than 50 monsters remaining (no exact number)
        //if 255 - not supported for current map (town or scenary map)
        public CharacterClass PlayerClass => (CharacterClass) (ServerDataStruct.PlayerClass & 0xF);
        public byte MonstersRemaining => ServerDataStruct.MonstersRemaining;
        public ushort CurrentSulphiteAmount => _cachedValue.Value.CurrentSulphiteAmount;
        public int CurrentAzuriteAmount => _cachedValue.Value.CurrentAzuriteAmount;

        #region PlayerData

        public ushort LastActionId => ServerDataStruct.LastActionId;
        public int CharacterLevel => ServerDataStruct.CharacterLevel;
        public int PassiveRefundPointsLeft => ServerDataStruct.PassiveRefundPointsLeft;
        public int FreePassiveSkillPointsLeft => ServerDataStruct.FreePassiveSkillPointsLeft;
        public int QuestPassiveSkillPoints => ServerDataStruct.QuestPassiveSkillPoints;
        public int TotalAscendencyPoints => ServerDataStruct.TotalAscendencyPoints;
        public int SpentAscendencyPoints => ServerDataStruct.SpentAscendencyPoints;
        public PartyAllocation PartyAllocationType => (PartyAllocation) ServerDataStruct.PartyAllocationType;
        public string League => ServerDataStruct.League.ToString(M);
        public PartyStatus PartyStatusType => (PartyStatus) this.ServerDataStruct.PartyStatusType;
        public bool IsInGame => NetworkState == NetworkStateE.Connected;
        public NetworkStateE NetworkState => (NetworkStateE) this.ServerDataStruct.NetworkState;
        public int Latency => ServerDataStruct.Latency;
        public string Guild => NativeStringReader.ReadString(M.Read<long>(Address + 0x65B8), M);

        public BetrayalData BetrayalData => GetObject<BetrayalData>(M.Read<long>(Address + 0x3C8, 0x718));

        public IList<ushort> SkillBarIds
        {
            get
            {
                if (Address == 0) return null;
                var res = new List<ushort>();

                var readAddr = _cachedValue.Value.SkillBarIds;
                res.Add(readAddr.SkillBar1);
                res.Add(readAddr.SkillBar2);
                res.Add(readAddr.SkillBar3);
                res.Add(readAddr.SkillBar4);
                res.Add(readAddr.SkillBar5);
                res.Add(readAddr.SkillBar6);
                res.Add(readAddr.SkillBar7);
                res.Add(readAddr.SkillBar8);
                return res;
            }
        }

        public IList<ushort> PassiveSkillIds
        {
            get
            {
                if (Address == 0) return null;
                var fisrPtr = ServerDataStruct.PassiveSkillIds.First;
                var endPtr = ServerDataStruct.PassiveSkillIds.Last;
                var total_stats = (int) (endPtr - fisrPtr);
                var bytes = M.ReadMem(fisrPtr, total_stats);
                var res = new List<ushort>();

                if (total_stats < 0 || total_stats > 500)
                    return null;
                for (var i = 0; i < bytes.Length; i += 2)
                {
                    var id = BitConverter.ToUInt16(bytes, i);
                    res.Add(id);
                }

                return res;
            }
        }

        #endregion

        private List<Player> result = new List<Player>();

        public IList<Player> NearestPlayers
        {
            get
            {
                if (Address == 0) return null;
                var startPtr = ServerDataStruct.NearestPlayers.First;
                var endPtr = ServerDataStruct.NearestPlayers.Last;
                startPtr += 16; //Don't ask me why. Just skipping first 2
                //Sometimes wrong offsets and read 10000000+ objects
                if (startPtr < Address || (endPtr - startPtr) / 16 > 50)
                    return result;
                result.Clear();
                for (var addr = startPtr; addr < endPtr; addr += 16) //16 because we are reading each second pointer (pointer vectors)
                    result.Add(ReadObject<Player>(addr));
                return result;
            }
        }

        #region Stash Tabs

        // public IList<ServerStashTab> PlayerStashTabs => GetStashTabs(Extensions.GetOffset<ServerDataOffsets>("PlayerStashTabsStart"), Extensions.GetOffset<ServerDataOffsets>("PlayerStashTabsEnd"));
        public IList<ServerStashTab> PlayerStashTabs =>
            GetStashTabs(Extensions.GetOffset<ServerDataOffsets>(nameof(ServerDataOffsets.PlayerStashTabs)),
                         Extensions.GetOffset<ServerDataOffsets>(nameof(ServerDataOffsets.PlayerStashTabs)) + 0x8);

        public IList<ServerStashTab> GuildStashTabs =>
            GetStashTabs(Extensions.GetOffset<ServerDataOffsets>(nameof(ServerDataOffsets.GuildStashTabs)),
                         Extensions.GetOffset<ServerDataOffsets>(nameof(ServerDataOffsets.GuildStashTabs)) + 0x8);

        private IList<ServerStashTab> GetStashTabs(int offsetBegin, int offsetEnd) {
            if (Address == 0) return null;
            var firstAddr = M.Read<long>(Address + offsetBegin);
            var lastAddr = M.Read<long>(Address + offsetEnd);
            var len = lastAddr - firstAddr;
            if (len <= 0 || len > 2048 || firstAddr <= 0 || lastAddr <= 0)
                return new List<ServerStashTab>();
            var tabs = M.ReadStructsArray<ServerStashTab>(firstAddr, lastAddr, ServerStashTab.StructSize, TheGame);


            //Skipping hidden tabs of premium maps tab (read notes in StashTabController.cs)
            //tabs.RemoveAll(x => (x.Flags & InventoryTabFlags.Hidden) == InventoryTabFlags.Hidden);
            return new List<ServerStashTab>(tabs);
            // return new List<IServerStashTab>(tabs.Where(x=> (x.Flags & InventoryTabFlags.Hidden) != 0));
        }

        #endregion

        #region Inventories

        public IList<InventoryHolder> PlayerInventories
        {
            get
            {
                if (Address == 0) return null;
                var firstAddr = ServerDataStruct.PlayerInventories.First;
                var lastAddr = ServerDataStruct.PlayerInventories.Last;
                //Sometimes wrong offsets and read 10000000+ objects
                if (firstAddr == 0 || (lastAddr - firstAddr) / InventoryHolder.StructSize > 1024)
                    return new List<InventoryHolder>();
                return M.ReadStructsArray<InventoryHolder>(firstAddr, lastAddr, InventoryHolder.StructSize, this).ToList();
            }
        }

        public IList<InventoryHolder> NPCInventories
        {
            get
            {
                if (Address == 0) return null;
                var firstAddr = ServerDataStruct.NPCInventories.First;
                var lastAddr = ServerDataStruct.NPCInventories.Last;
                //Sometimes wrong offsets and read 10000000+ objects
                if (firstAddr == 0 || (lastAddr - firstAddr) / InventoryHolder.StructSize > 1024)
                    return new List<InventoryHolder>();

                return M.ReadStructsArray<InventoryHolder>(firstAddr, lastAddr, InventoryHolder.StructSize, TheGame).ToList();
            }
        }

        public IList<InventoryHolder> GuildInventories
        {
            get
            {
                if (Address == 0) return null;
                var firstAddr = ServerDataStruct.GuildInventories.First;
                var lastAddr = ServerDataStruct.GuildInventories.Last;
                //Sometimes wrong offsets and read 10000000+ objects
                if (firstAddr == 0 || (lastAddr - firstAddr) / InventoryHolder.StructSize > 1024)
                    return new List<InventoryHolder>();
                return M.ReadStructsArray<InventoryHolder>(firstAddr, lastAddr, InventoryHolder.StructSize, TheGame).ToList();
            }
        }

        #region Utils functions

        public ServerInventory GetPlayerInventoryBySlot(InventorySlotE slot) {
            foreach (var inventory in PlayerInventories)
                if (inventory.Inventory.InventSlot == slot)
                    return inventory.Inventory;
            return null;
        }

        public ServerInventory GetPlayerInventoryByType(InventoryTypeE type) {
            foreach (var inventory in PlayerInventories)
                if (inventory.Inventory.InventType == type)
                    return inventory.Inventory;
            return null;
        }

        public ServerInventory GetPlayerInventoryBySlotAndType(InventoryTypeE type, InventorySlotE slot) {
            foreach (var inventory in PlayerInventories)
                if (inventory.Inventory.InventType == type && inventory.Inventory.InventSlot == slot)
                    return inventory.Inventory;
            return null;
        }

        #endregion

        #endregion

        #region Completed Areas

        public IList<WorldArea> UnknownAreas => GetAreas(0x6A88);
        public IList<WorldArea> CompletedAreas => GetAreas(0x6AC8);
        public IList<WorldArea> ShapedMaps => GetAreas(0x6B08);
        public IList<WorldArea> BonusCompletedAreas => GetAreas(0x6B48);
        public IList<WorldArea> ElderGuardiansAreas => GetAreas(0x6B88);
        public IList<WorldArea> MasterAreas => GetAreas(0x6BC8);

        public IList<WorldArea> ShaperElderAreas => GetAreas(0x6C08);
        //   public IList<WorldArea> _UniqCompletedMaps => GetAreas(0x6460); //Dunno what is this

        private IList<WorldArea> GetAreas(int offset) {
            if (Address == 0) return null;
            var res = new List<WorldArea>();
            if (Address == 0 || offset == 0)
                return res;
            var size = M.Read<int>(Address + offset - 0x8);
            var listStart = M.Read<long>(Address + offset);
            var error = 0;
            if (listStart == 0 || size == 0)
                return res;
            for (var addr = M.Read<long>(listStart); addr != listStart; addr = M.Read<long>(addr))
            {
                if (addr == 0) return res;
                var byAddress = TheGame.Files.WorldAreas.GetByAddress(M.Read<long>(addr + 0x18));
                if (byAddress != null)
                    res.Add(byAddress);
                if (--size < 0) break;
                error++;
                //Sometimes wrong offsets and read 10000000+ objects
                if (error > 1024)
                {
                    res = new List<WorldArea>();
                    break;
                }
            }

            return res;
        }

        #endregion


        public int GetBeastCapturedAmount(BestiaryCapturableMonster monster) => M.Read<int>(Address + 0x5240 + monster.Id * 4);
    }
}