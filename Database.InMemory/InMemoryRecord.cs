using System;
using System.Data;

namespace Database.InMemory
{
    public class InMemoryRecord
    {
        public DateTime LastModified { get; private set; }

        private object value;

        public InMemoryRecord (object value, DateTime lastModified)
        {
            this.value = value;
            LastModified = lastModified;
        }

        public T Get<T> () => (T)value;

        public void Update (InMemoryReference reference, object value)
        {
            if (reference.LastModified != LastModified)
                throw new DBConcurrencyException("Reference has missed record updates.");
            this.value = value;
            LastModified = reference.LastModified = DateTime.Now;
        }
    }
}
