using Microsoft.EntityFrameworkCore;

namespace Database.EntityFramework
{
    public class ContextFactory
    {
        protected virtual DbContextOptions ContextOptions { get; }

        public ContextFactory (DbContextOptions contextOptions)
        {
            ContextOptions = contextOptions;
        }

        public virtual DatabaseContext<T> Create<T> ()
            where T : class => new(ContextOptions);
    }
}
