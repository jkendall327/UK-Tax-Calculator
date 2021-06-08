using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace TaxCrud
{
    internal class DbHelper
    {
        internal SqliteConnection Connection { get; set; }

        internal DbHelper(string connectionString = "Data Source=mydb.db;")
        {
            Connection = new SqliteConnection(connectionString);
        }

        internal void Initialize()
        {
            Connection.Execute("CREATE TABLE IF NOT EXISTS Users " +
                "(Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "FirstName VARCHAR(20), " +
                "LastName VARCHAR(20));");

            Connection.Execute("CREATE TABLE IF NOT EXISTS Transactions " +
                "(Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Amount INTEGER, " +
                "Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP, " +
                "Transuser INTEGER, " +
                "FOREIGN KEY(transuser) REFERENCES Users(Id));");
        }

        internal const string GetUserDetails = "SELECT [Id], [FirstName],[LastName] FROM [Users]";

        internal void AddUser(string firstName, string lastName)
        {
            Connection.Execute("INSERT INTO Users (FirstName, LastName) VALUES (@fname,@lname)", new { fname = firstName, lname = lastName });
        }

        internal IEnumerable<Person> GetAllUsers()
        {
            return Connection.Query<Person>(GetUserDetails);
        }

        internal IEnumerable<Person> GetByName(string firstName, string lastName)
        {
            return Connection.Query<Person>($"{GetUserDetails} WHERE FirstName = @fname AND LastName = @lname", new { fname = firstName, lname = lastName });
        }

        internal void DeleteUser(int id)
        {
            Connection.Execute("DELETE FROM Users WHERE Id = @uid", new { uid = id });
        }

        internal void ResetDatabase()
        {
            Connection.Execute("DROP TABLE [Users];");
            Initialize();
        }

        internal Person GetByID(int result)
        {
            var person = Connection.Query<Person>($"{GetUserDetails} WHERE Id = @uid", new { uid = result }).Single();
            person.Transactions = GetTransactions(person.Id).ToList();

            return person;
        }

        internal void UpdateName(int id, string firstName, string lastName)
        {
            Connection.Execute($"UPDATE [Users] SET [FirstName] = '{firstName}', [LastName] = '{lastName}' WHERE [Id] = {id};");
        }

        internal void AddTransaction(int id, decimal amount)
        {
            Connection.Execute($"INSERT INTO [Transactions] (Amount, Transuser) VALUES (@am, @uid)", new { am = amount, uid = id });
        }

        internal IEnumerable<Transaction> GetTransactions(int id)
        {
            return Connection.Query<Transaction>("SELECT [Amount],[Timestamp] FROM [Transactions] WHERE [Transuser] = @uid", new { uid = id });
        }
    }
}
