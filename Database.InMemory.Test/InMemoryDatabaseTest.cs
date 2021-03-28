using Database.Test;

namespace Database.InMemory.Test
{
    public class InMemoryDatabaseTest : DatabaseTest
    {
        public InMemoryDatabaseTest ()
            : base(new InMemoryDatabase()) { }
    }
}
