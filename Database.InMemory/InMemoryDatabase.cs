using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Database.InMemory
{
    public class InMemoryDatabase : IDatabase
    {
        private readonly ConcurrentDictionary<Type,
            ConcurrentDictionary<string, InMemoryRecord>> recordsByType = new();

        public IReference<T> Add<T> (T record) where T : notnull
        {
            var reference = new InMemoryReference<T>();
            GetRecords<T>()[reference.Id] = new InMemoryRecord(record, reference.LastModified);
            return reference;
        }

        public T Get<T> (IReference<T> reference)
        {
            return GetRecord(reference).Get<T>();
        }

        public void Update<T> (IReference<T> reference, T record) where T : notnull
        {
            GetRecord(reference).Update((InMemoryReference)reference, record);
        }

        public void Remove<T> (IReference<T> reference)
        {
            var id = GetId(reference);
            if (!GetRecords<T>().TryRemove(id, out _))
                throw new KeyNotFoundException();
        }

        public IEnumerable<(IReference<T> Reference, T Record)> Query<T> ()
        {
            return GetRecords<T>().Select(CreateResult);

            static (IReference<T>, T) CreateResult (KeyValuePair<string, InMemoryRecord> kv)
            {
                var (id, record) = kv;
                return (new InMemoryReference<T>(id, record.LastModified), record.Get<T>());
            }
        }

        private static string GetId<T> (IReference<T> reference)
        {
            return ((InMemoryReference)reference).Id;
        }

        private InMemoryRecord GetRecord<T> (IReference<T> reference)
        {
            var id = GetId(reference);
            return GetRecords<T>()[id];
        }

        private ConcurrentDictionary<string, InMemoryRecord> GetRecords<T> ()
        {
            var type = typeof(T);
            if (!recordsByType.TryGetValue(type, out var records))
                recordsByType[type] = records = new ConcurrentDictionary<string, InMemoryRecord>();
            return records;
        }
    }
}
