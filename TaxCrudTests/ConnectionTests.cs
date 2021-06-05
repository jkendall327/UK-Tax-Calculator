using System.Linq;
using TaxCrud;
using Xunit;

namespace TaxCrudTests
{
    public class ConnectionTests
    {
        [Fact]
        public void ViewUsers()
        {
            var Connection = new Connection("Data Source=:memory:;");
            Connection.AddUser("Testman", "Test");

            Assert.Equal("Testman Test", Connection.GetAllUsers().First().ToString());
        }
    }
}
