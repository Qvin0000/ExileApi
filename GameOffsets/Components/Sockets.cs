namespace GameOffsets.Components
{
    using System.Runtime.InteropServices;
    using GameOffsets.Native;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Sockets
    {
        public ComponentHeader Header;
        public long UselessPtr;
        public SocketColors Colors; // 0 means no socket;
        public SocketGems Gems; // 0x0000 means there is no gem in the socket.
        public NativePtrArray ListsPtr; // Array of bytes. // Each byte store socket link size.
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SocketColors
    {
        public int S1;
        public int S2;
        public int S3;
        public int S4;
        public int S5;
        public int S6;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SocketGems
    {
        public long S1GemPtr; //SocketGemStructure
        public long S2GemPtr; //SocketGemStructure
        public long S3GemPtr; //SocketGemStructure
        public long S4GemPtr; //SocketGemStructure
        public long S5GemPtr; //SocketGemStructure
        public long S6GemPtr; //SocketGemStructure
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SocketGemStructure //Maybe this is a Gem component.
    {
        public ComponentHeader Header;
        public NativePtrArray Unknown0Ptr;
    }
}