namespace ExileCore.Shared
{
    public class MultiThreadProp<T> where T : class, new()
    {
        private T freeProp;
        private readonly object locker = new object();
        private T Prop1;
        private T Prop2;
        private int read;
        private int write;

        public MultiThreadProp()
        {
            Prop1 = new T();
            Prop2 = new T();
        }

        public bool CanUpdate => write - read < 3;
        public bool CanRead => write > read;

        public T Read()
        {
            lock (locker)
            {
                read++;
                return Prop1;
            }
        }

        public T Write()
        {
            lock (locker)
            {
                Swap();
                write++;
                return Prop1;
            }
        }

        private void Swap()
        {
            lock (locker)
            {
                freeProp = Prop1;
                Prop1 = Prop2;
                Prop2 = freeProp;
            }
        }
    }
}
