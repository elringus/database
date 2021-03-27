using System;
using System.Data;

namespace Database.InMemory
{
    public class InMemoryRecord
    {
        public DateTime LastModified { get; private set; }
        public object Value { get; private set; }

        public InMemoryRecord (object value, DateTime lastModified)
        {
            Value = value;
            LastModified = lastModified;
        }

        public T Get<T> () => (T)Value;

        public void Update (InMemoryReference reference, object value)
        {
            if (reference.LastModified != LastModified)
                throw new DBConcurrencyException("Reference has missed record updates.");
            Value = value;
            LastModified = reference.LastModified = DateTime.Now;
        }
    }
}
