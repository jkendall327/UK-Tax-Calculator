using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TaxCrudTests")]

namespace TaxCrud
{
    // store Person record
    // store Entry record for each financial transaction
    // determine total tax to pay

    class Program
    {
        static void Main() => new App().Run();
    }

    class App
    {
        public DbHelper Connection { get; set; } = new();

        public void Run()
        {
            Console.WriteLine("Tax Simulator 2021");

            Connection.Initialize();

            var mainConsole = new EasyConsole.Menu()
                .Add("View Users", ViewUsers)
                .Add("Add User", CreateUser)
                .Add("Delete User", DeleteUser)
                .Add("Search by name", SearchByName)
                .Add("Clear Database", ClearDatabase);

            while (true)
            {
                mainConsole.Display();
                Console.WriteLine(Environment.NewLine);
            }
        }

        private void ViewUsers()
        {
            try
            {
                var queryResult = Connection.GetAllUsers();

                if (!queryResult.Any())
                {
                    Console.WriteLine("No users!");
                }

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
            var fname = Prompt("Enter first name: ");
            var lname = Prompt("Enter last name: ");

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

        private void DeleteUser()
        {
            Console.WriteLine("Provide the ID of the user to delete.");

            if (!int.TryParse(Console.ReadLine(), out int result))
            {
                Console.WriteLine("Invalid ID. Please try again.");
                return;
            }

            var user = Connection.GetByID(result).FirstOrDefault();

            if (user is null)
            {
                Console.WriteLine("No user with that ID found. Returning.");
                return;
            }

            Console.WriteLine($"This will delete the user {user.Name}. Continue?");

            var yesOrNo = new EasyConsole.Menu()
                .Add($"Yes, delete {user.Name} permanently", () =>
                {
                    Connection.DeleteUser(result);
                    Console.WriteLine("User deleted.");
                })
                .Add("No, do not delete", () => Console.WriteLine("Aborting."));

            yesOrNo.Display();
        }

        private void SearchByName()
        {
            var fname = Prompt("Input first name: ");
            var lname = Prompt("Input last name: ");

            var results = Connection.GetByName(fname, lname);

            if (!results.Any())
            {
                Console.WriteLine("No users by that name found.");
                return;
            }

            foreach (var person in results)
            {
                Console.WriteLine(person.ToString());
            }
        }

        private void ClearDatabase()
        {
            Console.WriteLine("This will wipe all data, including users and financial transactions. Are you sure?");

            var yesOrNo = new EasyConsole.Menu()
                .Add("Yes, delete everything", () =>
                {
                    Connection.ResetDatabase();
                    Console.WriteLine("Database reset.");
                })
                .Add("No, keep my data", () => { Console.WriteLine("Aborting."); });

            yesOrNo.Display();
        }

        private string Prompt(string query)
        {
            Console.Write(query);
            return Console.ReadLine();
        }
    }
}
