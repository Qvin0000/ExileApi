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
        void SetApi(GameController gameController, Graphics graphics, PluginManager pluginManager);
        void OnPluginSelectedInMenu();//TODO: Implement me
        void EntityAdded(Entity entity);
        void EntityRemoved(Entity entity);
        void EntityAddedAny(Entity entity);
        void EntityIgnored(Entity entity);
        void AreaChange(AreaInstance area);
        void ReceiveEvent(string eventId, object args);
        void _LoadSettings();
        void _SaveSettings();
        void OnPluginDestroyForHotReload();
    }
}
