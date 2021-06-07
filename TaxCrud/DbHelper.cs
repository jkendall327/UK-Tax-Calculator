﻿using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

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
            Connection?.Execute("CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, FirstName VARCHAR(20), LastName VARCHAR(20));");
        }

        internal const string GetUserDetails = "SELECT [Id], [FirstName],[LastName] FROM [Users]";

        internal void AddUser(string firstName, string lastName)
        {
            Connection?.Execute("INSERT INTO Users (FirstName, LastName) VALUES (@fname,@lname)", new { fname = firstName, lname = lastName });
        }

        internal IEnumerable<Person> GetAllUsers()
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

        internal void UpdateName(int id, string firstName, string lastName)
        {
            Connection?.Execute($"UPDATE [Users] SET [FirstName] = '{firstName}', [LastName] = '{lastName}' WHERE [Id] = {id};");
        }
    }
}
