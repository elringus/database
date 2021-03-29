namespace Database.EntityFramework
{
    public class EntityFrameworkRecord<T> where T : class
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
        public int Id { get; private set; }
        public byte[] Timestamp { get; private set; }
        public T Record { get; set; }

        public EntityFrameworkRecord (int id, byte[] timestamp, T record)
        {
            Id = id;
            Timestamp = timestamp;
            Record = record;
        }
    }
}
