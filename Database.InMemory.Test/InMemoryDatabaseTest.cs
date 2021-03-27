using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace Database.InMemory.Test
{
    public class InMemoryDatabaseTest
    {
        private readonly InMemoryDatabase database;
        private readonly MockRecords records;

        public InMemoryDatabaseTest ()
        {
            database = new InMemoryDatabase();
            records = new MockRecords(database);
        }

        [Fact]
        public void AddReturnsReferenceOfRecordType ()
        {
            Assert.IsType<InMemoryReference<MockRecords.C>>(records.RC1);
        }

        [Fact]
        public void GetReturnsRecordEqualToAdded ()
        {
            Assert.Equal(records.A3, database.Get(records.RA3));
        }

        [Fact]
        public void UpdateModifiesRecord ()
        {
            database.Update(records.RA1, records.A3);
            Assert.Equal(records.A3, database.Get(records.RA1));
        }

        [Fact]
        public void WhenUpdatingMissingRecordExceptionIsThrown ()
        {
            database.Remove(records.RB2);
            Assert.Throws<KeyNotFoundException>(() => database.Update(records.RB2, records.B1));
        }

        [Fact]
        public void RemoveDeletesRecord ()
        {
            database.Remove(records.RB1);
            Assert.Throws<KeyNotFoundException>(() => database.Get(records.RB1));
        }

        [Fact]
        public void WhenRemovingMissingRecordExceptionIsThrown ()
        {
            database.Remove(records.RB2);
            Assert.Throws<KeyNotFoundException>(() => database.Remove(records.RB2));
        }

        [Fact]
        public void QueryReturnsMatchingRecord ()
        {
            var matchingRecord = database.Query<MockRecords.A>().First().Record;
            Assert.IsType<MockRecords.A>(matchingRecord);
        }

        [Fact]
        public void QueryReturnsMultipleMatchingRecords ()
        {
            var matchingRecords = database.Query<MockRecords.A>()
                .Where(q => q.Record.Integers.Length > 0)
                .Select(q => q.Record).ToArray();
            Assert.Equal(2, matchingRecords.Length);
            Assert.Contains(records.A1, matchingRecords);
            Assert.Contains(records.A3, matchingRecords);
        }

        [Fact]
        public void WhenNoMatchesQueryReturnsEmptyResult ()
        {
            var result = database.Query<MockRecords.C>()
                .Where(q => q.Record.BRecords.Length == 0);
            Assert.Empty(result);
        }

        [Fact]
        public void WhenUpdatingWithOutdatedReferenceExceptionIsThrown ()
        {
            var reference = database.FirstReference<MockRecords.A>(r => r.Id == "3");
            database.Update(records.RA3, records.A1);
            Assert.Throws<DBConcurrencyException>(() => database.Update(reference, records.A2));
        }

        [Fact]
        public void WhenUpdatingWithActualReferenceExceptionIsNotThrown ()
        {
            var reference = database.FirstReference<MockRecords.A>(r => r.Id == "3");
            database.Update(reference, records.A1);
            database.Update(reference, records.A2);
            Assert.Equal(records.A2, database.Get<MockRecords.A>(reference));
        }

        [Fact]
        public void WhenTransactWithVoidActionRecordsAreNotModified ()
        {
            database.Transact(() => { });
            Assert.False(IsAnyMockRecordModified());
        }

        private bool IsAnyMockRecordModified ()
        {
            return IsAnyMockRecordModified<MockRecords.A>() ||
                   IsAnyMockRecordModified<MockRecords.B>() ||
                   IsAnyMockRecordModified<MockRecords.C>();
        }

        private bool IsAnyMockRecordModified<T> () where T : MockRecords.IIdentifiable
        {
            foreach (var record in records.Records.OfType<T>())
                if (IsMockRecordModified<T>(record.Id))
                    return true;
            return false;
        }

        private bool IsMockRecordModified<T> (string id) where T : MockRecords.IIdentifiable
        {
            var databaseRecord = database.FirstRecord<T>(r => r.Id == id);
            var mockRecord = records.Records.OfType<T>().First(r => r.Id == id);
            return !databaseRecord.Equals(mockRecord);
        }
    }
}
