using System.Linq;
using TaxCrud;
using Xunit;

namespace TaxCrudTests
{
    public class ConnectionTests
    {
        internal DbHelper Connection { get; set; }

        public ConnectionTests()
        {
            Connection = new DbHelper("Data Source=:memory:;");

            // If we don't open the connection manually, Dapper will automatically open/close it when we call .Execute(), detroying the in-memory DB.
            Connection.Connection.Open();
            Connection.Initialize();
        }

        [Fact]
        public void AddUser_CorrectlyAddsUser_WhenGivenOneInput()
        {
            Connection.AddUser("John", "Smith");
            var results = Connection.GetAllUsers();

            Assert.Equal("John Smith", results.Single().Name);
        }

        [Fact]
        public void AddUser_CorrectlyAddsUsers_WhenGivenMultipleInputs()
        {
            Connection.AddUser("John", "Smith");
            Connection.AddUser("Jane", "Smith");
            var results = Connection.GetAllUsers();

            Assert.Equal(2, results.Count());
        }

        [Fact]
        public void DeleteUser_RemovesUser_WhenGivenCorrectID()
        {
            Connection.AddUser("John", "Smith");

            var john = Connection.GetAllUsers().Single();

            Connection.DeleteUser(john.Id);

            Assert.Empty(Connection.GetAllUsers());
        }

        [Fact]
        public void ResetDatabase_RemovesAllUsers_WhenOnlyOneUser()
        {
            // arrange
            Connection.AddUser("John", "Smith");

            // act
            Connection.ResetDatabase();

            // assert
            Assert.Empty(Connection.GetAllUsers());
        }


        [Fact]
        public void GetByID_ReturnsEqualPersonObject_WhenIDIsValid()
        {
            // arrange
            Connection.AddUser("John", "Smith");
            Connection.AddUser("Jane", "Doe");
            Connection.AddUser("Malcolm", "Gladwell");

            var expected = new Person() { FirstName = "Jane", LastName = "Doe", Id = 2 };

            // act
            var actual = Connection.GetByID(2);

            // assert
            // todo write custom equality comparer for person, breaks because List<> is reference type
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Balance, actual.Balance);
        }


        [Fact]
        public void GetById_ReturnsInvalidPerson_WhenIdIsInvalid()
        {
            var actual = Connection.GetByID(int.MaxValue);
            Assert.True(actual is InvalidPerson);
        }


        [Fact]
        public void UpdateName_UpdatesName_WhenIDIsValid()
        {
            // arrange
            Connection.AddUser("John", "Smith");

            // act
            Connection.UpdateName(1, "John", "Evans"); // he got married and took his partner's name

            // assert
            Assert.Equal("John Evans", Connection.GetByID(1).Name);
        }


        [Fact]
        public void GetByID_ReturnsPersonObjectWithCorrectTransactions_WhenGivenValidID()
        {
            // arrange
            Connection.AddUser("John", "Smith");
            Connection.AddTransaction(1, new Transaction(43.58m));
            Connection.AddTransaction(1, new Transaction(-11.28m));

            // act
            var person = Connection.GetByID(1);

            // assert
            Assert.Equal(32.3m, person.Balance);
        }


        [Fact]
        public void GetTransactions_ReturnsCorrectValues_WhenGivenValidID()
        {
            // arrange
            Connection.AddUser("John", "Smith");
            Connection.AddTransaction(1, new Transaction(43.58m));
            Connection.AddTransaction(1, new Transaction(-11.28m));

            // act
            var transactions = Connection.GetTransactions(1);

            // assert
            Assert.Equal(32.3m, transactions.Sum(x => x.Amount));
        }


        [Fact]
        public void AddTransaction_RoundsValues_WhenMoreThanTwoDecimalPlaces()
        {
            // arrange
            Connection.AddUser("John", "Smith");
            Connection.AddTransaction(1, new Transaction(43.5853453m)); // 43.59
            Connection.AddTransaction(1, new Transaction(-11.3433453m)); // -11.34

            // act
            var person = Connection.GetByID(1);

            // assert
            Assert.Equal(32.25m, person.Balance);
        }
    }
}
