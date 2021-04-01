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
            var entry = context.Records.Add(record);
            context.SaveChanges();
            var id = context.GetId(record);
            return new EntityFrameworkReference<T>(id);
        }

        public T Get<T> (IReference<T> reference) where T : class
        {
            var id = GetId(reference);
            return CreateContext<T>().Find<T>(id) ?? throw new NotFoundException();
        }

        public void Update<T> (IReference<T> reference, T record) where T : class
        {
            var id = GetId(reference);
            var context = CreateContext<T>();
            var storedRecord = context.Find<T>(id);
            if (storedRecord is null) throw new NotFoundException();
            if (!ReferenceEquals(record, storedRecord))
            {
                context.Records.Remove(storedRecord);
                context.Records.Add(record);
                context.SetId(record, id);
            }
            context.SaveChanges();
        }

        public void Remove<T> (IReference<T> reference) where T : class
        {
            var id = GetId(reference);
            var context = CreateContext<T>();
            var record = context.Find<T>(id);
            if (record is null) throw new NotFoundException();
            context.Records.Remove(record);
            context.SaveChanges();
        }

        public (IReference<T> Reference, T Record)? Find<T> (Predicate<T> predicate) where T : class
        {
            var context = CreateContext<T>();
            var record = context.Records.AsEnumerable().FirstOrDefault(r => predicate(r));
            if (record is null) return null;
            var id = context.GetId(record);
            var reference = new EntityFrameworkReference<T>(id);
            return (reference, record);
        }

        public ICollection<(IReference<T> Reference, T Record)> FindAll<T> (Predicate<T> predicate) where T : class
        {
            var context = CreateContext<T>();
            var records = context.Records.AsEnumerable().Where(r => predicate(r));
            return records.Select(CreateResult).ToList();

            (IReference<T>, T) CreateResult (T record)
            {
                var id = context.GetId(record);
                var reference = new EntityFrameworkReference<T>(id);
                return (reference, record);
            }
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
