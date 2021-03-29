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

        public IReference<T> Add<T> (T record) where T : class
        {
            throw new NotImplementedException();
        }

        public T Get<T> (IReference<T> reference) where T : class
        {
            throw new NotImplementedException();
        }

        public void Update<T> (IReference<T> reference, T record) where T : class
        {
            throw new NotImplementedException();
        }

        public void Remove<T> (IReference<T> reference) where T : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<(IReference<T> Reference, T Record)> Query<T> () where T : class
        {
            throw new NotImplementedException();
        }

        public ITransaction Transact (Action action)
        {
            throw new NotImplementedException();
        }

        private DatabaseContext<T> CreateContext<T> () where T : class => new(contextOptions);
    }
}
