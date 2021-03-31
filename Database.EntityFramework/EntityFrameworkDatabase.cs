using System;
using System.Collections.Generic;

namespace Database.EntityFramework
{
    public class EntityFrameworkDatabase : IDatabase
    {
        private readonly ContextFactory contextFactory;

        public EntityFrameworkDatabase (ContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public IReference<T> Add<T> (T record) where T : class
        {
            var context = CreateContext<T>();
            var entry = context.GetSet().Add(record);
            context.SaveChanges();
            var id = context.GetId(record);
            return new EntityFrameworkReference<T>(id);
        }

        public T Get<T> (IReference<T> reference) where T : class
        {
            var id = GetId(reference);
            return CreateContext<T>().FirstOrDefault(id) ?? throw new NotFoundException();
        }

        public void Update<T> (IReference<T> reference, T record) where T : class
        {
            var id = GetId(reference);
            var context = CreateContext<T>();
            var storedRecord = context.FirstOrDefault(id);
            if (storedRecord is null) throw new NotFoundException();
            if (!ReferenceEquals(record, storedRecord))
            {
                context.GetSet().Remove(storedRecord);
                context.GetSet().Add(record);
                context.SetId(record, id);
            }
            context.SaveChanges();
        }

        public void Remove<T> (IReference<T> reference) where T : class
        {
            var id = GetId(reference);
            var context = CreateContext<T>();
            var record = context.FirstOrDefault(id);
            if (record is null) throw new NotFoundException();
            context.GetSet().Remove(record);
            context.SaveChanges();
        }

        public IEnumerable<(IReference<T> Reference, T Record)> Query<T> () where T : class
        {
            throw new NotImplementedException();
        }

        public ITransaction Transact (Action action)
        {
            throw new NotImplementedException();
        }

        private static int GetId<T> (IReference<T> reference) where T : class
        {
            return ((EntityFrameworkReference<T>)reference).Id;
        }

        private DatabaseContext<T> CreateContext<T> () where T : class => contextFactory.Create<T>();
    }
}
