using FluentAssertions;
using System.Linq;
using TaxCrud;
using Xunit;

namespace TaxCrudTests
{
    public class ConnectionTests
    {
        internal SqliteHelper Connection { get; set; }

        public ConnectionTests()
        {
            Connection = new SqliteHelper("Data Source=:memory:;");

            // If we don't open the connection manually, Dapper will automatically open/close it when we call .Execute(), detroying the in-memory DB.
            Connection.Connection.Open();
            Connection.Initialize();
        }

        [Fact]
        public void AddUser_CorrectlyAddsUser_WhenGivenOneInput()
        {
            Connection.AddUser("John", "Smith");
            Connection.GetAllUsers().Should().HaveCount(1, "because one user was added");
        }

        [Fact]
        public void AddUser_CorrectlyAddsUsers_WhenGivenMultipleInputs()
        {
            Connection.AddUser("John", "Smith");
            Connection.AddUser("Jane", "Smith");

            Connection.GetAllUsers().Should().HaveCount(2, "because two users were added");
        }

        [Fact]
        public void DeleteUser_RemovesUser_WhenGivenCorrectID()
        {
            Connection.AddUser("John", "Smith");

            var john = Connection.GetAllUsers().Single();

            Connection.DeleteUser(john.Id);

            Connection.GetAllUsers().Should().BeEmpty("because the only user has been deleted");
        }

        [Fact]
        public void ResetDatabase_RemovesAllUsers_WhenOnlyOneUser()
        {
            // arrange
            Connection.AddUser("John", "Smith");

            // act
            Connection.ResetDatabase();

            // assert
            Connection.GetAllUsers().Should().BeEmpty("because resetting a database removes all users");
        }

        [Fact]
        public void ResetDatabase_RemovesAllUsers_WhenMultipleUsers()
        {
            // arrange
            Connection.AddUser("John", "Smith");
            Connection.AddUser("Jane", "Smith");
            Connection.AddUser("Alice", "Doe");

            // act
            Connection.ResetDatabase();

            // assert
            Connection.GetAllUsers().Should().BeEmpty("because resetting a database removes all users");
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
            Assert.Equal(expected, actual, new PersonComparer());
        }


        [Fact]
        public void GetById_ReturnsInvalidPerson_WhenIdIsInvalid()
        {
            Connection.AddUser("John", "Smith");
            var actual = Connection.GetByID(int.MaxValue);
            actual.Should().BeOfType<InvalidPerson>("because requests for invalid IDs return InvalidPerson objects");
        }


        [Fact]
        public void UpdateName_UpdatesName_WhenIDIsValid()
        {
            // arrange
            Connection.AddUser("John", "Smith");

            // act
            Connection.UpdateName(1, "John", "Evans"); // he got married and took his partner's name

            // assert
            Connection.GetByID(1).Name.Should().Be("John Evans", "because names should be updated when a valid ID is given");
        }

        [Fact]
        public void UpdateName_DoesNotUpdateName_WhenIDIsInvalid()
        {
            // arrange
            Connection.AddUser("John", "Smith");

            // act
            Connection.UpdateName(2, "John", "Evans");

            // assert
            Connection.GetByID(1).Name.Should().Be("John Smith", "because names should not be updated when an invalid ID is given");
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
            person.Balance.Should().Be(32.3m, "otherwise a user's transactions are not being retrieved or summed correctly");
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
            transactions.Sum(x => x.Amount).Should().Be(32.3m, "otherwise a user's transactions are not being retrieved or summed correctly");
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
            person.Balance.Should().Be(32.25m, "because transactions should round up");
        }
    }
}
