using System.Collections.Generic;

namespace Database.InMemory
{
    public class InMemoryTransaction : ITransaction
    {
        public static readonly ITransaction Completed = new InMemoryTransaction();
        
        private readonly Dictionary<InMemoryReference, InMemoryRecord?> snapshots = new();

        public void WaitForCompletion () { }

        public void Snapshot (InMemoryReference reference, InMemoryRecord? record)
        {
            if (snapshots.ContainsKey(reference)) return;
            snapshots[reference] = DeepClone.Copy(record) as InMemoryRecord;
        }

        public void Rollback (InMemoryStore store)
        {
            foreach (var (reference, record) in snapshots)
            {
                var records = store.GetRecords(reference.RecordType);
                if (record is null && records.ContainsKey(reference.Id))
                    records.Remove(reference.Id, out _);
                else if (record != null) records[reference.Id] = record;
            }
            snapshots.Clear();
        }
    }
}
