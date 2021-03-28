using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Database.Test
{
    public abstract class DatabaseTest
    {
        private readonly IDatabase database;
        private readonly MockRecords records;

        protected DatabaseTest (IDatabase database)
        {
            this.database = database;
            records = new MockRecords(database);
        }

        [Fact]
        public void AddReturnsReferenceOfRecordType ()
        {
            Assert.IsAssignableFrom<IReference<MockRecords.C>>(records.RC1);
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
            Assert.Throws<NotFoundException>(() => database.Update(records.RB2, records.B1));
        }

        [Fact]
        public void RemoveDeletesRecord ()
        {
            database.Remove(records.RB1);
            Assert.Null(database.FindRecord<MockRecords.B>(r => r.Id == "1"));
            Assert.Throws<NotFoundException>(() => database.Get(records.RB1));
        }

        [Fact]
        public void WhenRemovingMissingRecordExceptionIsThrown ()
        {
            database.Remove(records.RB2);
            Assert.Throws<NotFoundException>(() => database.Remove(records.RB2));
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
        public void WhenNotExactTypeQueryReturnsEmpty ()
        {
            Assert.Empty(database.Query<MockRecords.Record>());
        }

        [Fact]
        public void FindReturnsMatchingReferenceAndRecord ()
        {
            Assert.Equal(records.A2, database.FindRecord<MockRecords.A>(r => r.Id == "2"));
            Assert.Equal(records.RA2, database.FindReference<MockRecords.A>(r => r.Id == "2"));
        }

        [Fact]
        public void WhenNoMatchFindReturnsNull ()
        {
            Assert.Null(database.FindRecord<MockRecords.C>(r => r.Id == "0"));
            Assert.Null(database.FindReference<MockRecords.C>(r => r.Id == "0"));
        }

        [Fact]
        public void FirstReturnsMatchingReferenceAndRecord ()
        {
            Assert.Equal(records.A2, database.FirstRecord<MockRecords.A>(r => r.Id == "2"));
            Assert.Equal(records.RA2, database.FirstReference<MockRecords.A>(r => r.Id == "2"));
        }

        [Fact]
        public void WhenNoMatchFirstThrowsException ()
        {
            Assert.Throws<NotFoundException>(() => database.FirstRecord<MockRecords.C>(r => r.Id == "0"));
            Assert.Throws<NotFoundException>(() => database.FirstReference<MockRecords.C>(r => r.Id == "0"));
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
            Assert.Throws<ConcurrencyException>(() => database.Update(reference, records.A2));
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
            var transaction = database.Transact(() => { });
            transaction.WaitForCompletion();
            Assert.Equal(records.C1, database.Get<MockRecords.C>(records.RC1));
        }

        [Fact]
        public void TransactPerformsOperationsInOrder ()
        {
            var transaction = database.Transact(() => {
                database.Update(records.RA1, database.Get<MockRecords.A>(records.RA3));
                database.Remove(records.RA3);
                database.Update(records.RA2, database.Get<MockRecords.A>(records.RA1));
            });
            transaction.WaitForCompletion();
            Assert.Equal(records.A3, database.Get<MockRecords.A>(records.RA2));
            Assert.Throws<NotFoundException>(() => database.Get(records.RA3));
        }

        [Fact, ExcludeFromCodeCoverage]
        public void TransactionIsRolledBackOnException ()
        {
            try
            {
                database.Transact(() => {
                    database.Update(records.RA1, database.Get<MockRecords.A>(records.RA3));
                    database.Remove(records.RA3);
                    database.Update(records.RA2, database.Get<MockRecords.A>(records.RA1));
                    database.Add(records.C1);
                    throw new MockException();
                });
            }
            catch (MockException) { }
            Assert.Equal(records.A1.Id, database.Get<MockRecords.A>(records.RA1).Id);
            Assert.Equal(records.A2.Id, database.Get<MockRecords.A>(records.RA2).Id);
            Assert.Equal(records.A3.Id, database.Get<MockRecords.A>(records.RA3).Id);
            Assert.Single(database.Query<MockRecords.C>());
        }

        [Fact, ExcludeFromCodeCoverage]
        public void RollbackRestoresInitialRecordValues ()
        {
            try
            {
                database.Transact(() => {
                    var record = database.Get<MockRecords.C>(records.RC1);
                    record.BRecords[0] = records.RB2;
                    database.Update(records.RC1, record);
                    throw new MockException();
                });
            }
            catch (MockException) { }
            var reference = database.Get<MockRecords.C>(records.RC1).BRecords[0];
            Assert.Equal(records.B1, database.Get<MockRecords.B>(reference));
        }

        [Fact, ExcludeFromCodeCoverage]
        public void TransactionIsRetriedOnConcurrencyException ()
        {
            var invocations = 0;
            try
            {
                database.Transact(() => {
                    invocations++;
                    throw new ConcurrencyException();
                });
            }
            catch (ConcurrencyException) { }
            Assert.True(invocations > 1);
        }

        [Fact, ExcludeFromCodeCoverage]
        public void TransactionIsNotRetriedOnOtherExceptions ()
        {
            var invocations = 0;
            try
            {
                database.Transact(() => {
                    invocations++;
                    throw new MockException();
                });
            }
            catch (MockException) { }
            Assert.Equal(1, invocations);
        }
    }
}
