using FluentAssertions;
using System.Linq;
using TaxCrud;
using Xunit;

namespace TaxCrudTests
{
    public class SearchTests
    {
        internal SqliteHelper Connection { get; set; }

        public SearchTests()
        {
            Connection = new SqliteHelper("Data Source=:memory:;");

            // If we don't open the connection manually, Dapper will automatically open/close it when we call .Execute(), detroying the in-memory DB.
            Connection.Connection.Open();
            Connection.Initialize();
        }

        [Fact]
        public void GetByName_ShouldReturnCorrectUser_WhenCaseIsWrong()
        {
            // arrange
            Connection.AddUser("John", "Doe");

            // act
            var results = Connection.GetByName("john", "doe");

            // assert
            results.First().Id.Should().Be(1);
        }


        [Fact]
        public void GetByName_ShouldReturnCorrectUser_WhenNameIsMisspelled()
        {
            // arrange
            Connection.AddUser("John", "Doe");

            // act
            var results = Connection.GetByName("jonn", "deo");

            // assert
            results.First().Id.Should().Be(1);
        }


        [Fact]
        public void GetByName_ShouldReturnMultiplePossibilities_WhenMultipleUsersInDatabase()
        {
            // arrange
            Connection.AddUser("John", "Doe");
            Connection.AddUser("John", "Dean");
            Connection.AddUser("John", "Doe");
            Connection.AddUser("James", "Dean");
            Connection.AddUser("Malcolm", "Gladwell");

            // act
            var results = Connection.GetByName("jonn", "doe");

            // assert
            results.Should().HaveCount(3, "because all names except two are similar");
        }


        [Fact]
        public void GetByName_ShouldReturnNothing_WhenNoSimilarNamesExist()
        {
            // arrange
            Connection.AddUser("John", "Doe");
            Connection.AddUser("James", "Dean");
            Connection.AddUser("Malcolm", "Gladwell");

            // act
            var results = Connection.GetByName("Clarissa", "Crawford");

            // assert
            results.Should().BeEmpty("no names in the database are similar to the search term");
        }
    }
}
