using ImGuiNET;

namespace Exile.RenderQ
{
    public unsafe struct FontContainer
    {
        public ImFont* Atlas { get; }
        public string Name { get; }
        public int Size { get; }

        public FontContainer(ImFont* atlas, string Name, int Size) {
            Atlas = atlas;
            this.Name = Name;
            this.Size = Size;
        }
    }
}