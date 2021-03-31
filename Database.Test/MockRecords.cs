using System.Diagnostics.CodeAnalysis;

namespace Database.Test
{
    [ExcludeFromCodeCoverage]
    public class MockRecords
    {
        public abstract record Record (string Id);
        public record A (string Id, ValueCollection<int> Integers) : Record(Id);
        public record B (string Id, bool Bool, IReference<A> ARecord) : Record(Id);
        public record C (string Id, IReference<B>[] BRecords) : Record(Id);

        public readonly A A1, A2, A3;
        public readonly B B1, B2;
        public readonly C C1;

        public readonly IReference<A> RA1, RA2, RA3;
        public readonly IReference<B> RB1, RB2;
        public readonly IReference<C> RC1;

        public MockRecords (IDatabase database)
        {
            A1 = new("1", new ValueCollection<int> { 1, -2 });
            RA1 = database.Add(A1);

            A2 = new("2", new ValueCollection<int>());
            RA2 = database.Add(A2);

            A3 = new("3", new ValueCollection<int> { 0 });
            RA3 = database.Add(A3);

            B1 = new("1", false, RA1);
            RB1 = database.Add(B1);

            B2 = new("2", true, RA2);
            RB2 = database.Add(B2);

            C1 = new("1", new[] { RB1, RB2 });
            RC1 = database.Add(C1);
        }
    }
}
