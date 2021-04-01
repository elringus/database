using System;
using System.Collections.Generic;

namespace Database
{
    public interface IDatabase
    {
        IReference<T> Add<T> (T record) where T : class;
        T Get<T> (IReference<T> reference) where T : class;
        void Update<T> (IReference<T> reference, T record) where T : class;
        void Remove<T> (IReference<T> reference) where T : class;
        (IReference<T> Reference, T Record)? Find<T> (Predicate<T> predicate) where T : class;
        ICollection<(IReference<T> Reference, T Record)> FindAll<T> (Predicate<T> predicate) where T : class;
        ITransaction Transact (Action action);
    }
}
