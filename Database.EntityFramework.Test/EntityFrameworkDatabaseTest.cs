using Database.Test;

namespace Database.EntityFramework.Test
{
    public class EntityFrameworkDatabaseTest : DatabaseTest
    {
        public EntityFrameworkDatabaseTest ()
            : base(CreateMockDatabase()) { }

        private static EntityFrameworkDatabase CreateMockDatabase ()
        {
            var contextFactory = new MockContextFactory();
            return new EntityFrameworkDatabase(contextFactory);
        }
    }
}
