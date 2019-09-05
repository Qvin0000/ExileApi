using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using Exile;
using Shared.Helpers;
using Shared.Interfaces;
using GameOffsets;
using Shared.Enums;
using SharpDX;

namespace PoEMemory
{
    public class TheGame : RemoteMemoryObject
    {
        public TheGame(IMemory m, Cache cache) {
            pM = m;
            pCache = cache;
            pTheGame = this;
            Instance = this;
            Address = m.Read<long>(m.BaseOffsets[OffsetsName.GameStateOffset] + m.AddressOfProcess);
            _AreaChangeCount = new TimeCache<int>(() => M.Read<int>(M.AddressOfProcess + M.BaseOffsets[OffsetsName.AreaChangeCount]), 50);

            AllGameStates = ReadHashMap(Address + 0x48);

            PreGameStatePtr = AllGameStates["PreGameState"].Address;
            LoginStatePtr = AllGameStates["LoginState"].Address;
            SelectCharacterStatePtr = AllGameStates["SelectCharacterState"].Address;
            WaitingStatePtr = AllGameStates["WaitingState"].Address;
            InGameStatePtr = AllGameStates["InGameState"].Address;
            LoadingStatePtr = AllGameStates["LoadingState"].Address;
            EscapeStatePtr = AllGameStates["EscapeState"].Address;
            LoadingState = AllGameStates["AreaLoadingState"].AsObject<AreaLoadingState>();
            IngameState = AllGameStates["InGameState"].AsObject<IngameState>();
            _inGame = new FrameCache<bool>(
                () => IngameState.Address != 0 && IngameState.Data.Address != 0 && IngameState.ServerData.Address != 0 && !IsLoading /*&&
                                                 IngameState.ServerData.IsInGame*/);

            Files = new FilesContainer(m);

            var IngameStateOffsetsNameOf = new IngameStateOffsets();
            var IngameDataOffsetsNameOf = new IngameDataOffsets();
            DataOff = Extensions.GetOffset<IngameStateOffsets>(nameof(IngameStateOffsetsNameOf.Data));
            CurrentAreaHashOff = Extensions.GetOffset<IngameDataOffsets>(nameof(IngameDataOffsetsNameOf.CurrentAreaHash));
        }

        private bool initialized = false;

        public void Init() { }


        public FilesContainer Files { get; set; }

        private int DataOff;

        private int CurrentAreaHashOff;

        //I hope this caching will works fine
        private static long PreGameStatePtr = -1;
        private static long LoginStatePtr = -1;
        private static long SelectCharacterStatePtr = -1;
        private static long WaitingStatePtr = -1;
        private static long InGameStatePtr = -1;
        private static long LoadingStatePtr = -1;
        private static long EscapeStatePtr = -1;
        private static TheGame Instance;

        public readonly Dictionary<string, GameState> AllGameStates;
        public AreaLoadingState LoadingState { get; private set; }
        public IngameState IngameState { get; private set; }
        public IList<GameState> CurrentGameStates => M.ReadDoublePtrVectorClasses<GameState>(Address + 0x8, IngameState);
        public IList<GameState> ActiveGameStates => M.ReadDoublePtrVectorClasses<GameState>(Address + 0x20, IngameState, true);
        public bool IsPreGame => GameStateActive(PreGameStatePtr);
        public bool IsLoginState => GameStateActive(LoginStatePtr);
        public bool IsSelectCharacterState => GameStateActive(SelectCharacterStatePtr);
        public bool IsWaitingState => GameStateActive(WaitingStatePtr); //This happens after selecting character, maybe other cases
        public bool IsInGameState => GameStateActive(InGameStatePtr);   //In game, with selected character

        public bool IsLoadingState => GameStateActive(LoadingStatePtr);
        public bool IsEscapeState => GameStateActive(EscapeStatePtr);
        public bool IsLoading => LoadingState.IsLoading;

        private readonly CachedValue<bool> _inGame;
        private readonly CachedValue<int> _AreaChangeCount;
        public int AreaChangeCount => _AreaChangeCount.Value;
        public bool InGame => _inGame.Value;

        public uint CurrentAreaHash
        {
            get
            {
                var hash = M.Read<uint>(IngameState.Address + DataOff, CurrentAreaHashOff);
                return hash;
            }
        }

        private static bool GameStateActive(long stateAddress) {
            var gameStateController = Instance;
            if (gameStateController == null) return false;
            var M = gameStateController.M;
            var address = Instance.Address + 0x20;
            var start = M.Read<long>(address);
            //var end = Read<long>(address + 0x8);
            var last = M.Read<long>(address + 0x10);

            var length = (int) (last - start);
            var bytes = M.ReadMem(start, length);

            for (var readOffset = 0; readOffset < length; readOffset += 16)
            {
                var pointer = BitConverter.ToInt64(bytes, readOffset);
                if (stateAddress == pointer) return true;
            }

            return false;
        }

        private Dictionary<string, GameState> ReadHashMap(long pointer) {
            var result = new Dictionary<string, GameState>();

            var stack = new Stack<GameStateHashNode>();
            var startNode = ReadObject<GameStateHashNode>(pointer);
            var item = startNode.Root;
            stack.Push(item);

            while (stack.Count != 0)
            {
                var node = stack.Pop();
                if (!node.IsNull)
                    result[node.Key] = node.Value1;

                var prev = node.Previous;
                if (!prev.IsNull)
                    stack.Push(prev);

                var next = node.Next;
                if (!next.IsNull)
                    stack.Push(next);
            }

            return result;
        }

        private class GameStateHashNode : RemoteMemoryObject
        {
            public GameStateHashNode Previous => ReadObject<GameStateHashNode>(Address);
            public GameStateHashNode Root => ReadObject<GameStateHashNode>(Address + 0x8);

            public GameStateHashNode Next => ReadObject<GameStateHashNode>(Address + 0x10);

            //public readonly byte Unknown;
            public bool IsNull => M.Read<byte>(Address + 0x19) != 0;

            //private readonly byte byte_0;
            //private readonly byte byte_1;
            public string Key => M.ReadNativeString(Address + 0x20);

            //public readonly int Useless;
            public GameState Value1 => ReadObject<GameState>(Address + 0x40);
            //public readonly long Value2;
        }
    }

    public class GameState : RemoteMemoryObject
    {
        private string stateName;
        public string StateName => stateName ?? (stateName = M.ReadNativeString(Address + 0x10));

        public override string ToString() => StateName;
    }


    public class AreaLoadingState : GameState
    {
        //This is actualy pointer to loading screen stuff (image, etc), but should works fine.
        public bool IsLoading => M.Read<long>(Address + 0xD8) == 1;
        public string AreaName => M.ReadStringU(M.Read<long>(Address + 0x1F0));

        public override string ToString() => $"{AreaName}, IsLoading: {IsLoading}";
    }
}