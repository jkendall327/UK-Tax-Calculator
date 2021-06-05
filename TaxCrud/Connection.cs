using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace TaxCrud
{
    internal class Connection
    {
        public static SqliteConnection Get()
        {
            return new SqliteConnection(@"Data Source=mydb.db;");
        }

        public static void AddUser(string firstName, string lastName)
        {
            using var connection = Get();
            connection.Execute("INSERT INTO Users (FirstName, LastName) VALUES (@fname,@lname)", new { fname = firstName, lname = lastName });
        }

        public static IEnumerable<Person> GetAllUsers()
        {
            using var connection = Get();
            return connection.Query<Person>("SELECT [Id], [FirstName],[LastName] FROM [Users]");
        }
    }
}
