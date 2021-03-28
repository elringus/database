using Database.Test;
using Microsoft.EntityFrameworkCore;

namespace Database.EntityFramework.Test
{
    public class EntityFrameworkDatabaseTest : DatabaseTest
    {
        public EntityFrameworkDatabaseTest ()
            : base(new EntityFrameworkDatabase(CreateOptions())) { }

        private static DbContextOptions CreateOptions ()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase("MockDatabase");
            return optionsBuilder.Options;
        }
    }
}
