using System;

namespace Database.EntityFramework
{
    public abstract class EntityFrameworkReference
    {
        public int Id { get; }
        public abstract Type RecordType { get; }

        protected EntityFrameworkReference (int id)
        {
            Id = id;
        }
    }

    public class EntityFrameworkReference<T> : EntityFrameworkReference, IReference<T> where T : class
    {
        public override Type RecordType { get; } = typeof(T);

        public EntityFrameworkReference (int id) : base(id) { }
    }
}
