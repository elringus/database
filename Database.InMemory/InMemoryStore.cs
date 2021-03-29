using System;
using System.Collections.Concurrent;

namespace Database.InMemory
{
    public class InMemoryStore
    {
        private readonly ConcurrentDictionary<Type,
            ConcurrentDictionary<string, InMemoryRecord>> recordsByType = new();

        public ConcurrentDictionary<string, InMemoryRecord> GetRecords<T> () where T : class
        {
            return GetRecords(typeof(T));
        }

        public InMemoryRecord GetRecord<T> (IReference<T> reference) where T : class
        {
            var id = ((InMemoryReference)reference).Id;
            var records = GetRecords<T>();
            if (records.TryGetValue(id, out var record))
                return record;
            throw new NotFoundException();
        }

        public ConcurrentDictionary<string, InMemoryRecord> GetRecords (Type type)
        {
            if (!recordsByType.TryGetValue(type, out var records))
                recordsByType[type] = records = new ConcurrentDictionary<string, InMemoryRecord>();
            return records;
        }
    }
}
