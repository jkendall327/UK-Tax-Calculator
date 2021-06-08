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
                new Transaction(){ Amount = 54.32m },
                new Transaction(){ Amount = -28.11m }
            };

            person.Transactions = transactions;

            // assert
            Assert.Equal(26.21m, person.Balance);
        }
    }
}
