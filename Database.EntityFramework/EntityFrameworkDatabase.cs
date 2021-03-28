using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Database.EntityFramework
{
    public class EntityFrameworkDatabase : IDatabase
    {
        private readonly DbContextOptions contextOptions;

        public EntityFrameworkDatabase (DbContextOptions contextOptions)
        {
            this.contextOptions = contextOptions;
        }

        public IReference<T> Add<T> (T record) where T : notnull
        {
            throw new NotImplementedException();
        }

        public T Get<T> (IReference<T> reference)
        {
            throw new NotImplementedException();
        }

        public void Update<T> (IReference<T> reference, T record) where T : notnull
        {
            throw new NotImplementedException();
        }

        public void Remove<T> (IReference<T> reference)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<(IReference<T> Reference, T Record)> Query<T> ()
        {
            throw new NotImplementedException();
        }

        public ITransaction Transact (Action action)
        {
            throw new NotImplementedException();
        }

        private DbContext CreateContext () => new(contextOptions);
    }
}
