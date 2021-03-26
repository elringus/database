using System;
using System.Diagnostics.CodeAnalysis;

namespace Database.InMemory.Test
{
    [ExcludeFromCodeCoverage]
    public class MockRecords
    {
        public record A (string String, int[] Integers);
        public record B (bool Bool, IReference<A> ARecord);
        public record C (IReference<B>[] BRecords);

        public readonly A A1, A2, A3;
        public readonly B B1, B2;
        public readonly C C1;

        public readonly IReference<A> RA1, RA2, RA3;
        public readonly IReference<B> RB1, RB2;
        public readonly IReference<C> RC1;

        public MockRecords (IDatabase database)
        {
            A1 = new("Lorem ipsum", new[] { 1, -2 });
            RA1 = database.Add(A1);

            A2 = new(" ", Array.Empty<int>());
            RA2 = database.Add(A2);

            A3 = new(string.Empty, new[] { 0 });
            RA3 = database.Add(A3);

            B1 = new(false, RA1);
            RB1 = database.Add(B1);

            B2 = new(true, RA2);
            RB2 = database.Add(B2);

            C1 = new(new[] { RB1, RB2 });
            RC1 = database.Add(C1);
        }
    }
}
