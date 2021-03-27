using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Database.InMemory.Test
{
    [ExcludeFromCodeCoverage]
    public class MockRecords
    {
        public interface IIdentifiable
        {
            string Id { get; }
        }

        public record A (string Id, int[] Integers) : IIdentifiable;
        public record B (string Id, bool Bool, IReference<A> ARecord) : IIdentifiable;
        public record C (string Id, IReference<B>[] BRecords) : IIdentifiable;

        public readonly A A1, A2, A3;
        public readonly B B1, B2;
        public readonly C C1;
        public readonly IReadOnlyCollection<IIdentifiable> Records;

        public readonly IReference<A> RA1, RA2, RA3;
        public readonly IReference<B> RB1, RB2;
        public readonly IReference<C> RC1;

        public MockRecords (IDatabase database)
        {
            var records = new List<IIdentifiable>();

            A1 = new("1", new[] { 1, -2 });
            records.Add(A1);
            RA1 = database.Add(A1);

            A2 = new("2", Array.Empty<int>());
            records.Add(A2);
            RA2 = database.Add(A2);

            A3 = new("3", new[] { 0 });
            records.Add(A3);
            RA3 = database.Add(A3);

            B1 = new("1", false, RA1);
            records.Add(B1);
            RB1 = database.Add(B1);

            B2 = new("2", true, RA2);
            records.Add(B2);
            RB2 = database.Add(B2);

            C1 = new("1", new[] { RB1, RB2 });
            records.Add(C1);
            RC1 = database.Add(C1);

            Records = records;
        }
    }
}
