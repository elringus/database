using System;

namespace Database.InMemory
{
    public class InMemoryReference : IEquatable<InMemoryReference>
    {
        public string Id { get; }
        public DateTime LastModified { get; set; }

        public InMemoryReference ()
            : this(Guid.NewGuid().ToString(), DateTime.Now) { }

        public InMemoryReference (string id, DateTime lastModified)
        {
            Id = id;
            LastModified = lastModified;
        }

        public bool Equals (InMemoryReference? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
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
            return Id.GetHashCode();
        }
    }

    public class InMemoryReference<T> : InMemoryReference, IReference<T>
    {
        public InMemoryReference () { }

        public InMemoryReference (string id, DateTime lastModified)
            : base(id, lastModified) { }
    }
}
