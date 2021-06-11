using Dapper;
using EasyConsole;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace TaxCrud
{
    internal class SqliteHelper : IDatabaseHelper
    {
        public DbConnection Connection { get; set; }

        internal SqliteHelper(string connectionString = "Data Source=mydb.db;")
        {
            Connection = new SqliteConnection(connectionString);
        }

        public void Initialize()
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

        public Person GetPerson()
        {
            // TODO: get the highest ID in the database here?
            var desiredID = Input.ReadInt(1, int.MaxValue);

            var person = GetByID(desiredID);

            if (person is InvalidPerson)
            {
                Console.WriteLine($"No user with ID {desiredID} found. Returning.");
                Console.ReadLine();
            }

            return person;
        }

        public void AddUser(string firstName, string lastName)
        {
            Connection.Execute("INSERT INTO Users (FirstName, LastName) VALUES (@fname,@lname)", new { fname = firstName, lname = lastName });
        }

        public IEnumerable<Person> GetAllUsers()
        {
            var people = Connection.Query<Person>(GetUserDetails);

            foreach (var person in people)
            {
                person.Transactions = GetTransactions(person.Id).ToList();
            }

            return people;
        }

        public IEnumerable<Person> GetByName(string firstName, string lastName)
        {
            return Connection.Query<Person>($"{GetUserDetails} WHERE FirstName = @fname AND LastName = @lname", new { fname = firstName, lname = lastName });
        }

        public void DeleteUser(int id)
        {
            Connection.Execute("DELETE FROM Users WHERE Id = @uid", new { uid = id });
        }

        public void ResetDatabase()
        {
            Connection.Execute("DROP TABLE [Transactions];");
            Connection.Execute("DROP TABLE [Users];");

            Initialize();
        }

        public Person GetByID(int result)
        {
            var person = Connection.Query<Person>($"{GetUserDetails} WHERE Id = @uid", new { uid = result }).SingleOrDefault();

            if (person is null) return new InvalidPerson();

            person.Transactions = GetTransactions(person.Id).ToList();

            return person;
        }

        public void UpdateName(int id, string firstName, string lastName)
        {
            Connection.Execute($"UPDATE [Users] SET [FirstName] = '{firstName}', [LastName] = '{lastName}' WHERE [Id] = {id};");
        }

        public void AddTransaction(int id, Transaction trans)
        {
            Connection.Execute($"INSERT INTO [Transactions] (Amount, Transuser) VALUES (@am, @uid)", new { am = trans.Amount, uid = id });
        }

        public IEnumerable<Transaction> GetTransactions(int id)
        {
            return Connection.Query<Transaction>("SELECT [Amount],[Timestamp] FROM [Transactions] WHERE [Transuser] = @uid", new { uid = id });
        }
    }
}
