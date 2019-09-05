namespace Shared.Interfaces
{
    public interface IPattern
    {
        string Name { get; }
        byte[] Bytes { get; }
        string Mask { get; }
        int StartOffset { get; }
    }
}