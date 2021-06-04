using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace TaxCrud
{
    // store Person record
    // store Entry record for each financial transaction
    // determine total tax to pay

    class Program
    {
        static void Main(string[] args) => new App().Run();
    }

    class App
    {
        readonly Dictionary<int, Action> Actions = new();

        public void Run()
        {
            Console.WriteLine("Tax Simulator 2021");

            FillDictionary();

            var connection = Connection.Get();

            // make tables...

            connection.Close();

            while (true)
            {
                Console.Write("> ");

                var response = Console.ReadLine();

                // parse response
                if (int.TryParse(response, out int result) == false) { Actions[0].Invoke(); continue; }

                // match response with action
                if (Actions.TryGetValue(result, out var action) == false) { Actions[0].Invoke(); continue; }

                action.Invoke();
            }
        }

        private void FillDictionary()
        {
            Actions.Add(0, InvalidAction);
            Actions.Add(1, ViewUsers);
            Actions.Add(2, CreateUser);
        }

        private void InvalidAction() => Console.WriteLine("Invalid response. Please try again.");

        private void ViewUsers()
        {
            using var connection = Connection.Get();

            try
            {
                var queryResult = connection.Query<Person>("SELECT [Id], [FirstName],[LastName] FROM dbo.[Users]");

                foreach (var person in queryResult)
                {
                    Console.WriteLine(person);
                }
            }
            catch (SqliteException)
            {
                Console.WriteLine("No users found");
            }
        }

        private void CreateUser()
        {
            using var connection = Connection.Get();

        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    class Connection
    {
        public static SqliteConnection Get()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            return connection;
        }
    }
}
