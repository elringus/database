using System;
using System.Collections.Generic;
using System.Linq;

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
            return new EntityFrameworkReference<T>(context.GetId(record));
        }

        public T Get<T> (IReference<T> reference) where T : class
        {
            var id = GetId(reference);
            var context = CreateContext<T>();
            var result = context.GetSet().FirstOrDefault(r => context.GetId(r) == id);
            return result ?? throw new NotFoundException();
        }

        public void Update<T> (IReference<T> reference, T record) where T : class
        {
            var id = GetId(reference);
            var context = CreateContext<T>();
            var set = context.GetSet();
            var storedRecord = set.FirstOrDefault(r => context.GetId(r) == id);
            if (storedRecord is null) throw new NotFoundException();
            if (!ReferenceEquals(record, storedRecord))
            {
                set.Remove(storedRecord);
                set.Add(record);
            }
            context.SaveChanges();
        }

        public void Remove<T> (IReference<T> reference) where T : class
        {
            var id = GetId(reference);
            var context = CreateContext<T>();
            var set = context.GetSet();
            var record = set.FirstOrDefault(r => context.GetId(r) == id);
            if (record is null) throw new NotFoundException();
            set.Remove(record);
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
