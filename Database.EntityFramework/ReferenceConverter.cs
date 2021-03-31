using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database.EntityFramework
{
    public class ReferenceConverter : ValueConverter
    {
        public override Type ModelClrType { get; }
        public override Type ProviderClrType { get; } = typeof(string);
        public override Func<object?, object> ConvertToProvider { get; }
        public override Func<object, object?> ConvertFromProvider { get; }

        private const char separator = '|';

        public ReferenceConverter (Type propertyType) : base(
            (Expression<Func<object?, object>>)(v => SerializeProperty(v)),
            (Expression<Func<object, object?>>)(v => DeserializeProperty(v)))
        {
            ModelClrType = propertyType;
            ConvertToProvider = SerializeProperty;
            ConvertFromProvider = DeserializeProperty;
        }

        private static string SerializeProperty (object? obj)
        {
            if (!(obj is IEnumerable collection)) return SerializeReference(obj);
            return $"[{string.Join(",", collection.OfType<object>().Select(SerializeReference))}]";
        }

        private static object? DeserializeProperty (object obj)
        {
            if (!(obj is string reference))
                throw new Exception($"Deserialization of reference type `{obj.GetType()}` is not supported.");
            if (!reference.StartsWith('[')) return DeserializeReference(reference);
            return reference.Substring(1, reference.Length - 2).Split(',').Select(DeserializeReference).ToArray();
        }

        private static string SerializeReference (object? obj)
        {
            if (obj is null) return string.Empty;
            if (!(obj is EntityFrameworkReference reference))
                throw new Exception($"Serialization of reference type `{obj.GetType()}` is not supported.");
            return $"{reference.Id}{separator}{reference.GetType().AssemblyQualifiedName}";
        }

        private static object? DeserializeReference (string reference)
        {
            if (string.IsNullOrEmpty(reference)) return null;
            var separatorIndex = reference.IndexOf(separator);
            var id = int.Parse(reference.Substring(0, separatorIndex));
            var recordType = Type.GetType(reference.Substring(separatorIndex + 1));
            if (recordType is null) throw new Exception($"Failed to create `{reference}` record type.");
            var referenceType = typeof(EntityFrameworkReference<>).MakeGenericType(recordType);
            var value = Activator.CreateInstance(recordType, id) as EntityFrameworkReference;
            return value ?? throw new Exception($"Failed to deserialize `{reference}` reference.");
        }
    }
}
