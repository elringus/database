using System;
using System.Collections.Generic;

namespace Database
{
    public interface IDatabase
    {
        IReference<T> Add<T> (T record) where T : notnull;
        T Get<T> (IReference<T> reference) where T : notnull;
        void Update<T> (IReference<T> reference, T record) where T : notnull;
        void Remove<T> (IReference<T> reference) where T : notnull;
        IEnumerable<(IReference<T> Reference, T Record)> Query<T> () where T : notnull;
        ITransaction Transact (Action action);
    }
}
