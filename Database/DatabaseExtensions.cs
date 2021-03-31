using System;

namespace Database
{
    public static class DatabaseExtensions
    {
        public static (IReference<T> Reference, T Record)? Find<T> (this IDatabase database, Predicate<T> predicate) where T : class
        {
            foreach (var (reference, record) in database.Query<T>())
                if (predicate.Invoke(record))
                    return (reference, record);
            return null;
        }

        public static (IReference<T> Reference, T Record) First<T> (this IDatabase database, Predicate<T> predicate) where T : class
        {
            return Find(database, predicate) ?? throw new NotFoundException();
        }
    }
}
