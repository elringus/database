using System;
using System.Collections.Generic;
using System.Linq;

namespace Database.InMemory
{
    public class InMemoryDatabase : IDatabase
    {
        private readonly InMemoryStore store = new();
        private readonly object transactLock = new();
        private InMemoryTransaction? transaction;

        public IReference<T> Add<T> (T record) where T : class
        {
            var reference = new InMemoryReference<T>();
            store.GetRecords<T>()[reference.Id] = new InMemoryRecord(record, reference.LastModified);
            transaction?.Snapshot(reference, null);
            return reference;
        }

        public T Get<T> (IReference<T> reference) where T : class
        {
            var inMemoryReference = (InMemoryReference)reference;
            var storedRecord = store.GetRecord(reference);
            transaction?.Snapshot(inMemoryReference, storedRecord);
            return storedRecord.Get<T>();
        }

        public void Update<T> (IReference<T> reference, T record) where T : class
        {
            var inMemoryReference = (InMemoryReference)reference;
            var storedRecord = store.GetRecord(reference);
            transaction?.Snapshot(inMemoryReference, storedRecord);
            storedRecord.Update(inMemoryReference, record);
        }

        public void Remove<T> (IReference<T> reference) where T : class
        {
            var inMemoryReference = (InMemoryReference)reference;
            if (!store.GetRecords<T>().TryRemove(inMemoryReference.Id, out var record))
                throw new NotFoundException();
            transaction?.Snapshot(inMemoryReference, record);
        }

        public (IReference<T> Reference, T Record)? Find<T> (Predicate<T> predicate) where T : class
        {
            var (id, record) = store.GetRecords<T>().FirstOrDefault(r => predicate(r.Value.Get<T>()));
            if (record is null) return null;
            return (new InMemoryReference<T>(id, record.LastModified), record.Get<T>());
        }

        public ICollection<(IReference<T> Reference, T Record)> FindAll<T> (Predicate<T> predicate) where T : class
        {
            return store.GetRecords<T>().Where(r => predicate(r.Value.Get<T>())).Select(CreateResult).ToList();

            static (IReference<T>, T) CreateResult (KeyValuePair<string, InMemoryRecord> kv)
            {
                var (id, record) = kv;
                return (new InMemoryReference<T>(id, record.LastModified), record.Get<T>());
            }
        }

        public ITransaction Transact (Action action)
        {
            lock (transactLock)
            {
                transaction = new InMemoryTransaction();
                TryTransactWithRetry(action, 3);
                transaction = null;
                return InMemoryTransaction.Completed;
            }
        }

        private void TryTransactWithRetry (Action action, int retryLimit)
        {
            var retriesCount = 0;
            while (true)
                try
                {
                    action.Invoke();
                    return;
                }
                catch (ConcurrencyException)
                {
                    transaction?.Rollback(store);
                    if (retriesCount > 1) throw;
                    retriesCount++;
                }
                catch
                {
                    transaction?.Rollback(store);
                    throw;
                }
        }
    }
}
