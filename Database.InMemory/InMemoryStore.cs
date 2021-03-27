using System;
using System.Collections.Concurrent;

namespace Database.InMemory
{
    public class InMemoryStore
    {
        private readonly ConcurrentDictionary<Type,
            ConcurrentDictionary<string, StoredRecord>> recordsByType = new();

        public ConcurrentDictionary<string, StoredRecord> GetRecords<T> ()
        {
            return GetRecords(typeof(T));
        }

        public StoredRecord GetRecord<T> (IReference<T> reference)
        {
            var id = ((InMemoryReference)reference).Id;
            var records = GetRecords<T>();
            if (records.TryGetValue(id, out var record))
                return record;
            throw new NotFoundException();
        }

        public ConcurrentDictionary<string, StoredRecord> GetRecords (Type type)
        {
            if (!recordsByType.TryGetValue(type, out var records))
                recordsByType[type] = records = new ConcurrentDictionary<string, StoredRecord>();
            return records;
        }
    }
}
