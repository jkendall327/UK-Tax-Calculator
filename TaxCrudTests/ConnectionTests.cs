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

            Assert.Equal("John Smith", results.First().ToString());
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

            var JohnSmith = Connection.GetByName("John Smith");

            Connection.DeleteUser(JohnSmith.Id);

            Assert.Empty(Connection.GetAllUsers());
        }
    }
}
