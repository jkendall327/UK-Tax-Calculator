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

    internal class App
    {
        internal DbHelper Connection { get; set; } = new();

        internal void Run()
        {
            Console.WriteLine("Tax Simulator 2021");

            Connection.Initialize();

            var mainConsole = new EasyConsole.Menu()
                .Add("View Users", ViewUsers)
                .Add("Add User", CreateUser)
                .Add("Update User Name", UpdateName)
                .Add("Delete User", DeleteUser)
                .Add("Search by name", SearchByName)
                .Add("Clear Database", ClearDatabase)
                .Add("Exit", () => Environment.Exit(0));

            while (true)
            {
                mainConsole.Display();
                Console.WriteLine(Environment.NewLine);
            }
        }

        /// <summary>
        /// Parses user's input to see if it's a valid <see cref="Person.Id"/>.
        /// </summary>
        /// <returns>A <see cref="Person"/> if ID is valid; otherwise an <see cref="InvalidPerson"/> representing failure.</returns>
        private Person GetPerson()
        {
            if (!int.TryParse(Console.ReadLine(), out int result))
            {
                Console.WriteLine("Invalid ID. Please try again.");
                return new InvalidPerson();
            }

            var person = Connection.GetByID(result);

            if (person is null)
            {
                Console.WriteLine("No user with that ID found. Returning.");
                return new InvalidPerson();
            }

            return person;
        }

        /// <summary>
        /// Simplifies prompting a user for input.
        /// </summary>
        /// <param name="query">Message to display to user.</param>
        /// <returns>The user's input.</returns>
        private string Prompt(string query)
        {
            Console.Write(query);
            return Console.ReadLine();
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

        private void ViewUsers()
        {
            var queryResult = Connection.GetAllUsers();

            if (!queryResult.Any()) Console.WriteLine("No users!");

            foreach (var person in queryResult) Console.WriteLine(person);
        }

        private void UpdateName()
        {
            Console.WriteLine("Which user's name would you like to update?");

            var person = GetPerson();
            if (person is InvalidPerson) return;

            Console.WriteLine("Provide new name for user.");
            var fname = Prompt("Input first name: ");
            var lname = Prompt("Input last name: ");

            Console.WriteLine($"This change will affect user {person.Name}. Continue?");

            var yesOrNo = new EasyConsole.Menu()
                .Add($"Yes, update {person.Name}'s name to {fname} {lname}", () =>
                {
                    Connection.UpdateName(person.Id, fname, lname);
                    Console.WriteLine("User name updated.");
                })
                .Add("No, do not update name", () => Console.WriteLine("Aborting."));

            yesOrNo.Display();
        }

        private void DeleteUser()
        {
            Console.WriteLine("Provide the ID of the user to delete.");

            var person = GetPerson();
            if (person is InvalidPerson) return;

            Console.WriteLine($"This will delete the user {person.Name}. Continue?");

            var yesOrNo = new EasyConsole.Menu()
                .Add($"Yes, delete {person.Name} permanently", () =>
                {
                    Connection.DeleteUser(person.Id);
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
    }
}
