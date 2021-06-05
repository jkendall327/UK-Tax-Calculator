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
            var conn = new Connection("Data Source=:memory:;");
            conn.Initialize();
            conn.AddUser("Testman", "Test");

            Assert.Equal("Testman Test", conn.GetAllUsers().First().ToString());
        }
    }
}
