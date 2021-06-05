using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace TaxCrud
{
    internal class Connection
    {
        public string ConnectionString { get; set; }

        public Connection(string connectionString = "Data Source=mydb.db;")
        {
            ConnectionString = connectionString;
        }

        public void Initialize()
        {
            using var connection = Get();

            connection.Execute("CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, FirstName VARCHAR(20), LastName VARCHAR(20));");
        }

        public SqliteConnection Get()
        {
            return new SqliteConnection(ConnectionString);
        }

        public void AddUser(string firstName, string lastName)
        {
            using var connection = Get();
            connection.Execute("INSERT INTO Users (FirstName, LastName) VALUES (@fname,@lname)", new { fname = firstName, lname = lastName });
        }

        public IEnumerable<Person> GetAllUsers()
        {
            using var connection = Get();
            return connection.Query<Person>("SELECT [Id], [FirstName],[LastName] FROM [Users]");
        }
    }
}
