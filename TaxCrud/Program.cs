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

            foreach (var item in Actions)
            {
                Console.Write(item.Key + ") ");
                Console.WriteLine(item.Value.Method.ToString());
            }

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
                var queryResult = Connection.GetAllUsers();

                foreach (var person in queryResult)
                {
                    Console.WriteLine(person);
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Database error occured: {ex.Message}");
            }
        }

        private void CreateUser()
        {
            Console.Write("Enter first name: ");
            var fname = Console.ReadLine();

            Console.Write("Enter last name: ");
            var lname = Console.ReadLine();

            try
            {
                Connection.AddUser(fname, lname);
                Console.WriteLine($"New user {fname} {lname} added succesfully.");
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Database error occured: {ex.Message}");
            }
            finally
            {
                ViewUsers();
            }
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }

    class Connection
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
