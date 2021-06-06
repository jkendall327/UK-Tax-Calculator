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
        public void CanAddUser()
        {
            Connection.AddUser("John", "Smith");
            var results = Connection.GetAllUsers();

            Assert.Equal("John Smith", results.First().Name);
        }

        [Fact]
        public void CanAddMultipleUsers()
        {
            Connection.AddUser("John", "Smith");
            Connection.AddUser("Jane", "Smith");
            var results = Connection.GetAllUsers();

            Assert.Equal(2, results.Count());
        }

        [Fact]
        public void CanDeleteUser()
        {
            Connection.AddUser("John", "Smith");

            var john = Connection.GetAllUsers().First();

            Connection.DeleteUser(john.Id);

            Assert.Empty(Connection.GetAllUsers());
        }

        [Fact]
        public void CanResetDatabase()
        {
            // arrange
            Connection.AddUser("John", "Smith");

            // act
            Connection.ResetDatabase();

            // assert
            Assert.Empty(Connection.GetAllUsers());
        }


        [Fact]
        public void CanGetByID()
        {
            // arrange
            Connection.AddUser("John", "Smith");
            Connection.AddUser("Jane", "Doe");
            Connection.AddUser("Malcolm", "Gladwell");

            var expected = new Person() { FirstName = "Jane", LastName = "Doe", Id = 2 };

            // act
            var actual = Connection.GetByID(2).First();

            // assert
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Id, actual.Id);
        }
    }
}
