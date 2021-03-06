using FluentAssertions;
using System.Collections.Generic;
using TaxCrud;
using Xunit;

namespace TaxCrudTests
{
    public class PersonTests
    {

        [Fact]
        public void TransactionsSumCorrectly()
        {
            // arrange
            var person = new Person();
            var transactions = new List<Transaction>
            {
                new Transaction(54.32m),
                new Transaction(-28.11m)
            };

            person.Transactions = transactions;

            // assert
            person.Balance.Should().Be(26.21m);
        }
    }
}
