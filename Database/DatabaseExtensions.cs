using System;

namespace Database
{
    public static class DatabaseExtensions
    {
        public static T? FindRecord<T> (this IDatabase database, Predicate<T> predicate) where T : class
        {
            var result = Find(database, predicate);
            return result.HasValue ? result.Value.Record : default;
        }

        public static IReference<T>? FindReference<T> (this IDatabase database, Predicate<T> predicate) where T : class
        {
            return Find(database, predicate)?.Reference;
        }

        public static T FirstRecord<T> (this IDatabase database, Predicate<T> predicate) where T : class
        {
            var result = Find(database, predicate);
            return result.HasValue ? result.Value.Record : throw new NotFoundException("Record with the specified predicate it not found.");
        }

        public static IReference<T> FirstReference<T> (this IDatabase database, Predicate<T> predicate) where T : class
        {
            return Find(database, predicate)?.Reference ?? throw new NotFoundException("Record with the specified predicate it not found.");
        }

        public static (IReference<T> Reference, T Record)? Find<T> (this IDatabase database, Predicate<T> predicate) where T : class
        {
            foreach (var (reference, record) in database.Query<T>())
                if (predicate.Invoke(record))
                    return (reference, record);
            return null;
        }
    }
}
