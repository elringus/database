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
        IEnumerable<(IReference<T> Reference, T Record)> Query<T> () where T : class;
        ITransaction Transact (Action action);
    }
}
