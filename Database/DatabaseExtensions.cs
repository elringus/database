using System;

namespace Database
{
    public static class DatabaseExtensions
    {
        public static T? FindRecord<T> (this IDatabase database, Predicate<T> predicate) where T : notnull
        {
            return Find(database, predicate)?.Record;
        }

        public static IReference<T>? FindReference<T> (this IDatabase database, Predicate<T> predicate) where T : notnull
        {
            return Find(database, predicate)?.Reference;
        }

        public static T FirstRecord<T> (this IDatabase database, Predicate<T> predicate) where T : notnull
        {
            return Find(database, predicate)?.Record ?? throw new Exception("Record with the specified predicate it not found.");
        }

        public static IReference<T> FirstReference<T> (this IDatabase database, Predicate<T> predicate) where T : notnull
        {
            return Find(database, predicate)?.Reference ?? throw new Exception("Record with the specified predicate it not found.");
        }

        public static (IReference<T> Reference, T Record)? Find<T> (this IDatabase database, Predicate<T> predicate) where T : notnull
        {
            foreach (var (reference, record) in database.Query<T>())
                if (predicate.Invoke(record))
                    return (reference, record);
            return null;
        }
    }
}
