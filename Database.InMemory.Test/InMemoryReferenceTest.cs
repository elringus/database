using System;
using Database.Test;

namespace Database.InMemory.Test
{
    public class InMemoryReferenceEquatableTest : EquatableTest<InMemoryReference>
    {
        protected override InMemoryReference<MockRecords.A> EqualInstanceA { get; } = new("0", DateTime.MinValue);
        protected override InMemoryReference<MockRecords.A> EqualInstanceB { get; } = new("0", DateTime.MaxValue);
    }
}
