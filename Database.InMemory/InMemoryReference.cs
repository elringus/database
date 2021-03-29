using System;

namespace Database.InMemory
{
    public abstract class InMemoryReference : IEquatable<InMemoryReference>
    {
        public string Id { get; }
        public abstract Type RecordType { get; }
        public DateTime LastModified { get; set; }

        protected InMemoryReference ()
            : this(Guid.NewGuid().ToString(), DateTime.Now) { }

        protected InMemoryReference (string id, DateTime lastModified)
        {
            Id = id;
            LastModified = lastModified;
        }

        public bool Equals (InMemoryReference? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && RecordType == other.RecordType;
        }

        public override bool Equals (object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InMemoryReference)obj);
        }

        public override int GetHashCode ()
        {
            return HashCode.Combine(Id, RecordType);
        }
    }

    public class InMemoryReference<T> : InMemoryReference, IReference<T> where T : class
    {
        public override Type RecordType { get; } = typeof(T);

        public InMemoryReference () { }

        public InMemoryReference (string id, DateTime lastModified)
            : base(id, lastModified) { }
    }
}
