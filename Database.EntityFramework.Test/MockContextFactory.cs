using System.Text.Json;
using Database.Test;
using Microsoft.EntityFrameworkCore;

namespace Database.EntityFramework.Test
{
    public class MockContextFactory : ContextFactory
    {
        public MockContextFactory () : base(CreateOptions()) { }

        public override DatabaseContext<T> Create<T> () where T : class
        {
            if (typeof(T) == typeof(MockRecords.A))
                return (new AContext(ContextOptions) as DatabaseContext<T>)!;
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

                // InMemory provider doesn't support collections of primitive types.
                // https://github.com/dotnet/efcore/issues/11926
                modelBuilder.Entity<MockRecords.A>()
                    .Property(e => e.Integers)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, null),
                        v => JsonSerializer.Deserialize<ValueCollection<int>>(v, null)!);
            }
        }
    }
}
