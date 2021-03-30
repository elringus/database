using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database.EntityFramework
{
    public class ReferenceConverter : ValueConverter<EntityFrameworkReference?, string>
    {
        private const char separator = '|';

        public ReferenceConverter ()
            : base(v => Serialize(v), v => Deserialize(v)) { }

        private static string Serialize (EntityFrameworkReference? reference)
        {
            if (reference is null) return string.Empty;
            return $"{reference.Id}{separator}{reference.GetType().AssemblyQualifiedName}";
        }

        private static EntityFrameworkReference? Deserialize (string reference)
        {
            if (string.IsNullOrEmpty(reference)) return null;
            var separatorIndex = reference.IndexOf(separator);
            var id = int.Parse(reference.Substring(0, reference.Length - separatorIndex - 1));
            var recordType = Type.GetType(reference.Substring(separatorIndex + 1));
            if (recordType is null) throw new Exception($"Failed to create `{reference}` record type.");
            var referenceType = typeof(EntityFrameworkReference<>).MakeGenericType(recordType);
            var result = Activator.CreateInstance(recordType, id) as EntityFrameworkReference;
            return result ?? throw new Exception($"Failed to deserialize `{reference}` reference.");
        }
    }
}
