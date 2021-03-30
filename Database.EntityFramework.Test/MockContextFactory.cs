using System;
using System.Linq;
using System.Text.Json;
using Database.Test;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Database.EntityFramework.Test
{
    public class MockContextFactory : ContextFactory
    {
        public MockContextFactory () : base(CreateOptions()) { }

        public override DatabaseContext<T> Create<T> () where T : class
        {
            var type = typeof(T);
            if (type == typeof(MockRecords.A)) return (new AContext(ContextOptions) as DatabaseContext<T>)!;
            // if (type == typeof(MockRecords.B)) throw new NotImplementedException();
            // if (type == typeof(MockRecords.C)) throw new NotImplementedException();
            return base.Create<T>();
        }

        private static DbContextOptions CreateOptions ()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase("MockDatabase");
            return optionsBuilder.Options;
        }

        private class AContext : DatabaseContext<MockRecords.A>
        {
            public AContext (DbContextOptions options) : base(options) { }

            protected override void OnModelCreating (ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<MockRecords.A>()
                    .Property(e => e.Integers)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, null),
                        v => JsonSerializer.Deserialize<int[]>(v, null)!,
                        new ValueComparer<int[]>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToArray()));
            }
        }
    }
}
