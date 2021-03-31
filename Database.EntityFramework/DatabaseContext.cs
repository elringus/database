using System;
using System.Collections;
using System.Linq;
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

        public virtual void SetId (T record, int id)
        {
            Entry(record).Property(IdProperty).CurrentValue = id;
        }

        public virtual T? FirstOrDefault (int id)
        {
            return GetSet().FirstOrDefault(r => EF.Property<int>(r, IdProperty) == id);
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
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            foreach (var property in entity.GetProperties())
                if (IsReference(property.ClrType))
                    property.SetValueConverter(new ReferenceConverter(property.ClrType));
        }

        protected static bool IsReference (Type type)
        {
            if (type.IsArray) return IsReference(type.GetElementType()!);
            if (!type.IsGenericType) return false;
            if (typeof(IEnumerable).IsAssignableFrom(type))
                return IsReference(type.GetGenericArguments()[0]);
            return type.GetGenericTypeDefinition() == typeof(IReference<>);
        }
    }
}
