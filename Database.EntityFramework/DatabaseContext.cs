using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.EntityFramework
{
    public class DatabaseContext<T> : DbContext where T : class
    {
        public DatabaseContext (DbContextOptions options)
            : base(options) { }

        public DbSet<EntityFrameworkRecord<T>> GetSet () => Set<EntityFrameworkRecord<T>>();

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            Entity().HasKey(e => e.Id);
            Entity().Property(e => e.Timestamp).IsRowVersion();
            Entity().OwnsOne(e => e.Record);

            EntityTypeBuilder<EntityFrameworkRecord<T>> Entity () => modelBuilder.Entity<EntityFrameworkRecord<T>>();
        }
    }
}
