using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

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

        public void AddUser(string firstName, string lastName)
        {
            Connection?.Execute("INSERT INTO Users (FirstName, LastName) VALUES (@fname,@lname)", new { fname = firstName, lname = lastName });
        }

        public IEnumerable<Person> GetAllUsers()
        {
            return Connection?.Query<Person>("SELECT [Id], [FirstName],[LastName] FROM [Users]");
        }

        internal Person GetByName(string name)
        {
            string[] names = name.Split(' ');
            var result = Connection?.Query<Person>("SELECT [Id], [FirstName],[LastName] FROM [Users] WHERE FirstName = @fname AND LastName = @lname", new { fname = names[0], lname = names[1] }).First();
            return result;
        }

        internal void DeleteUser(object id)
        {
            Connection?.Execute("DELETE FROM Users WHERE Id = @uid", new { uid = id });
        }

        internal void ResetDatabase()
        {
            Connection?.Execute("DROP TABLE [Users];");
            Initialize();
        }
    }
}
