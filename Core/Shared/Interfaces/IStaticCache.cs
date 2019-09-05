using System;
using System.Collections.Generic;

namespace Shared.Interfaces
{
    public interface IStaticCache<T>
    {
        int Count { get; }
        int DeletedCache { get; }
        int ReadCache { get; }
        int ReadMemory { get; }
        string CoeffString { get; }
        float Coeff { get; }
        T Read(string addr, Func<T> func);
        void UpdateCache();

        bool Remove(string key);
    }
}