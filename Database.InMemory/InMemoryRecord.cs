using System;
using System.Data;

namespace Database.InMemory
{
    public class InMemoryRecord
    {
        public DateTime LastModified => lastModified;

        private object value;
        private DateTime lastModified;

        public InMemoryRecord (object value, DateTime lastModified)
        {
            this.value = value;
            this.lastModified = lastModified;
        }

        public T Get<T> () => (T)value;

        public void Update (InMemoryReference reference, object value)
        {
            if (reference.LastModified != lastModified)
                throw new DBConcurrencyException("Reference has missed record updates.");
            this.value = value;
            lastModified = reference.LastModified = DateTime.Now;
        }
    }
}
