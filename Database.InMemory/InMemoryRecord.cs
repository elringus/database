using System;

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

        public T Get<T> () where T : class => (T)Value;

        public void Update (InMemoryReference reference, object record)
        {
            if (reference.LastModified != LastModified)
                throw new ConcurrencyException("Reference has missed record updates.");
            Value = record;
            LastModified = reference.LastModified = DateTime.Now;
        }
    }
}
