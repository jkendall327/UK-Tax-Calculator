using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace TaxCrud
{
    internal class DbHelper
    {
        public SqliteConnection Connection { get; set; }

        public DbHelper(string connectionString = "Data Source=mydb.db;")
        {
            Connection = new SqliteConnection(connectionString);
        }

        public void Initialize()
        {
            Connection?.Execute("CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, FirstName VARCHAR(20), LastName VARCHAR(20));");
        }

        const string GetUserDetails = "SELECT [Id], [FirstName],[LastName] FROM [Users]";

        public void AddUser(string firstName, string lastName)
        {
            Connection?.Execute("INSERT INTO Users (FirstName, LastName) VALUES (@fname,@lname)", new { fname = firstName, lname = lastName });
        }

        public IEnumerable<Person> GetAllUsers()
        {
            return Connection?.Query<Person>(GetUserDetails);
        }

        internal IEnumerable<Person> GetByName(string firstName, string lastName)
        {
            return Connection?.Query<Person>($"{GetUserDetails} WHERE FirstName = @fname AND LastName = @lname", new { fname = firstName, lname = lastName });
        }

        internal void DeleteUser(int id)
        {
            Connection?.Execute("DELETE FROM Users WHERE Id = @uid", new { uid = id });
        }

        internal void ResetDatabase()
        {
            Connection?.Execute("DROP TABLE [Users];");
            Initialize();
        }

        internal IEnumerable<Person> GetByID(int result)
        {
            return Connection.Query<Person>($"{GetUserDetails} WHERE Id = @uid", new { uid = result });
        }
    }
}
