namespace Shared
{
    public class CoroutineTask<T> : ICoroutine
    {
        public void Continue() { }
        public bool IsCompleted { get; private set; }
        public ICoroutine[] Children { get; }

        public CoroutineTask() { }
    }

    public interface ICoroutine
    {
        void Continue();
        bool IsCompleted { get; }
        ICoroutine[] Children { get; }
    }
}