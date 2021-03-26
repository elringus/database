using System;
using Xunit;

namespace Database.Test
{
    public abstract class EquatableTest<T> where T : IEquatable<T>
    {
        protected abstract T EqualInstanceA { get; }
        protected abstract T EqualInstanceB { get; }

        [Fact]
        public void InstanceNotEqualToObjectOfDifferentType ()
        {
            Assert.False(EqualInstanceA.Equals(true));
        }

        [Fact]
        public void NullIsNotEqualToInstance ()
        {
            Assert.False(EqualInstanceA.Equals(default));
        }

        [Fact]
        public void NullIsNotEqualToInstanceObject ()
        {
            Assert.False(EqualInstanceA.Equals(default(object)));
        }

        [Fact]
        public void SameInstancesAreEqual ()
        {
            Assert.Equal(EqualInstanceA, EqualInstanceA);
        }

        [Fact]
        public void SameInstanceObjectsAreEqual ()
        {
            Assert.True(EqualInstanceA.Equals((object)EqualInstanceA));
        }

        [Fact]
        public void InstancesAreEqual ()
        {
            Assert.Equal(EqualInstanceA, EqualInstanceB);
        }

        [Fact]
        public void InstanceObjectsAreEqual ()
        {
            Assert.True(EqualInstanceA.Equals((object)EqualInstanceB));
        }

        [Fact]
        public void InstancesHashCodesAreEqual ()
        {
            Assert.Equal(EqualInstanceA.GetHashCode(), EqualInstanceB.GetHashCode());
        }
    }
}
