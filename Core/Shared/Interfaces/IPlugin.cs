using System;
using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.Shared.Interfaces
{
    public interface IPlugin : IDisposable
    {
        bool Initialized { get; set; }
        ISettings _Settings { get; }
        bool CanUseMultiThreading { get; }
        bool Force { get; }
        string DirectoryName { get; set; }
        string DirectoryFullName { get; set; }
        string InternalName { get; }
        string Name { get; }
        string Description { get; }
        int Order { get; }
        void DrawSettings();
        void OnLoad();
        void OnUnload();
        bool Initialise();
        Job Tick();
        void Render();
        void OnClose();
        void SetApi(object gameController, object graphics);
        void OnPluginSelectedInMenu();
        void EntityAdded(Entity Entity);
        void EntityRemoved(Entity Entity);
        void EntityAddedAny(Entity Entity);
        void EntityIgnored(Entity Entity);
        void AreaChange(AreaInstance area);
        void _LoadSettings();
        void _SaveSettings();
        void LogMsg(string msg);
        void OnPluginDestroyForHotReload();
    }
}
