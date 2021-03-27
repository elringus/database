using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Database.InMemory
{
    public class InMemoryDatabase : IDatabase
    {
        private readonly InMemoryStore store = new();
        private readonly object transactLock = new();
        private InMemoryTransaction? transaction;

        public IReference<T> Add<T> (T record) where T : notnull
        {
            var reference = new InMemoryReference<T>();
            store.GetRecords<T>()[reference.Id] = new InMemoryRecord(record, reference.LastModified);
            return reference;
        }

        public T Get<T> (IReference<T> reference) where T : notnull
        {
            return store.GetRecord(reference).Get<T>();
        }

        public void Update<T> (IReference<T> reference, T record) where T : notnull
        {
            store.GetRecord(reference).Update((InMemoryReference)reference, record);
        }

        public void Remove<T> (IReference<T> reference) where T : notnull
        {
            var id = ((InMemoryReference)reference).Id;
            if (!store.GetRecords<T>().TryRemove(id, out _))
                throw new KeyNotFoundException();
        }

        public IEnumerable<(IReference<T> Reference, T Record)> Query<T> () where T : notnull
        {
            return store.GetRecords<T>().Select(CreateResult);

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
                }
                catch (DBConcurrencyException)
                {
                    transaction?.Rollback(store);
                    if (retriesCount > 3) throw;
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
