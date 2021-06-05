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
        public void Run()
        {
            Console.WriteLine("Tax Simulator 2021");

            Connection.Initialize();

            var mainConsole = new EasyConsole.Menu()
                .Add("View Users", ViewUsers)
                .Add("Add User", CreateUser);

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

        private string Prompt(string query)
        {
            Console.Write(query);
            return Console.ReadLine();
        }
    }
}
