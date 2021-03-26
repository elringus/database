using System.Collections.Generic;

namespace Database
{
    public interface IDatabase
    {
        IReference<T> Add<T> (T record) where T : notnull;
        T Get<T> (IReference<T> reference);
        void Update<T> (IReference<T> reference, T record) where T : notnull;
        void Remove<T> (IReference<T> reference);
        IEnumerable<(IReference<T> Reference, T Record)> Query<T> ();
        // ITransaction Transact (Action action);
    }
}
