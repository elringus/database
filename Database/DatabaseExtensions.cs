using System.Collections.Generic;

namespace Database
{
    public static class DatabaseExtensions
    {
        public static (IReference<T> Reference, T Record)? GetFirst<T> (this IDatabase database) where T : class
        {
            return database.Find<T>(r => true);
        }

        public static ICollection<(IReference<T> Reference, T Record)> GetAll<T> (this IDatabase database) where T : class
        {
            return database.FindAll<T>(r => true);
        }
    }
}
