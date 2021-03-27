﻿using System;
using System.Collections.Concurrent;

namespace Database.InMemory
{
    public class InMemoryStore
    {
        private readonly ConcurrentDictionary<Type,
            ConcurrentDictionary<string, InMemoryRecord>> recordsByType = new();

        public ConcurrentDictionary<string, InMemoryRecord> GetRecords<T> ()
        {
            return GetRecords(typeof(T));
        }

        public InMemoryRecord GetRecord<T> (IReference<T> reference)
        {
            var id = ((InMemoryReference)reference).Id;
            return GetRecords<T>()[id];
        }

        public ConcurrentDictionary<string, InMemoryRecord> GetRecords (Type type)
        {
            if (!recordsByType.TryGetValue(type, out var records))
                recordsByType[type] = records = new ConcurrentDictionary<string, InMemoryRecord>();
            return records;
        }
    }
}
