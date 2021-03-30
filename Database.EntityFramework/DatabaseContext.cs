using System;
using Microsoft.EntityFrameworkCore;

namespace Database.EntityFramework
{
    public class DatabaseContext<T> : DbContext where T : class
    {
        protected virtual string IdProperty { get; } = "EFDbId";
        protected virtual string TimestampProperty { get; } = "EFDbTimestamp";

        public DatabaseContext (DbContextOptions options)
            : base(options) { }

        public virtual DbSet<T> GetSet () => Set<T>();

        public virtual int GetId (T record)
        {
            return (int)Entry(record).Property(IdProperty).CurrentValue;
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            CreatePrimaryKey(modelBuilder);
            CreateTimestamp(modelBuilder);
            DiscoverReferences(modelBuilder);
            ConvertReferences(modelBuilder);
        }

        protected virtual void CreatePrimaryKey (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T>().Property<int>(IdProperty);
            modelBuilder.Entity<T>().HasKey(IdProperty);
        }

        protected virtual void CreateTimestamp (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T>().Property<byte[]>(TimestampProperty).IsRowVersion();
        }

        protected virtual void DiscoverReferences (ModelBuilder modelBuilder)
        {
            foreach (var property in typeof(T).GetProperties())
                if (IsReference(property.PropertyType))
                    modelBuilder.Entity<T>().Property(property.Name);
        }

        protected virtual void ConvertReferences (ModelBuilder modelBuilder)
        {
            var converter = new ReferenceConverter();
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            foreach (var property in entity.GetProperties())
                if (IsReference(property.ClrType))
                    property.SetValueConverter(converter);
        }

        protected static bool IsReference (Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReference<>);
        }
    }
}
