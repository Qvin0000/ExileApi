using System;
using GameOffsets.Native;
using Shared.Helpers;

namespace PoEMemory.Components
{
    public class Base : Component
    {
        //x20 - some strings about item
        private string _name;
        public string Name => _name ?? (_name = M.Read<NativeStringU>(Address + 0x10, 0x18).ToString(M));


        public int ItemCellsSizeX => M.Read<int>(Address + 0x10, 0x10);
        public int ItemCellsSizeY => M.Read<int>(Address + 0x10, 0x14);
        public bool isCorrupted => M.Read<byte>(Address + 0xD8) == 1;
        public bool isShaper => M.Read<byte>(Address + 0xD9) == 1;

        public bool isElder => M.Read<byte>(Address + 0xDA) == 1;
        // public bool isFractured => M.Read<byte>(Address + 0x98) == 0;

        // 0x8 - link to base item
        // +0x10 - Name
        // +0x30 - Use hint
        // +0x50 - Link to Data/BaseItemTypes.dat

        // 0xC (+4) fileref to visual identity
    }
}