using Microsoft.Data.Sqlite;
using Spectre.Console;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TaxCrudTests")]

namespace TaxCrud
{
    class Program
    {
        static void Main() => new App(new SqliteHelper()).Run();
    }

    internal class App
    {
        internal IDatabaseHelper Connection { get; set; }

        public App(IDatabaseHelper connection)
            => Connection = connection ?? throw new ArgumentNullException(nameof(connection));

        internal void Run()
        {
            Console.WriteLine("Tax Simulator 2021");

            Connection.Initialize();

            var mainConsole = new EasyConsole.Menu()
                .Add("View User Details", ViewUserDetails)
                .Add("View All Users", ViewUsers)
                .Add("Add User", CreateUser)
                .Add("Update User Name", UpdateName)
                .Add("Delete User", DeleteUser)
                .Add("Search by name", SearchByName)
                .Add("Add transaction", AddTransaction)
                .Add("Clear Database", ClearDatabase)
                .Add("Exit", () => Environment.Exit(0));

            while (true)
            {
                mainConsole.Display();
            }
        }

        private Table GetUserColumns()
        {
            return new Table()
                .AddColumn("Id")
                .AddColumn("Name")
                .AddColumn("Current balance")
                .AddColumn("Outstanding tax (past year)");
        }

        private string[] GetUserRow(Person person)
        {
            return new string[] { person.Id.ToString(), person.Name, person.Balance.ToString("#,##0.00"), person.TaxOverLastYear().ToString("#,##0.00") };
        }

        private void ViewUserDetails()
        {
            Console.WriteLine("See details for which user?");

            var person = GetPerson();
            if (person is InvalidPerson) return;

            var table = GetUserColumns().AddRow(GetUserRow(person));
            AnsiConsole.Render(table);

            Console.WriteLine("Most recent transactions:");

            if (person.Transactions.IsEmpty()) return;

            var transactions = new Table()
                .AddColumn("Timestamp")
                .AddColumn("Amount");

            foreach (var transaction in person.Transactions.TakeLast(5))
            {
                transactions.AddRow(new string[] { transaction.Timestamp.ToString(), transaction.Amount.ToString() });
            }

            AnsiConsole.Render(transactions);
        }

        /// <summary>
        /// Parses user's input to see if it's a valid <see cref="Person.Id"/>.
        /// </summary>
        /// <returns>A <see cref="Person"/> if ID is valid; otherwise an <see cref="InvalidPerson"/> representing failure.</returns>
        private Person GetPerson()
        {
            var desiredID = DemandValidInt();

            var person = Connection.GetByID(desiredID);

            if (person is InvalidPerson) Console.WriteLine($"No user with ID {desiredID} found. Returning.");

            return person;
        }

        // repeats until user gives input that satisfies TryParse()
        private int DemandValidInt()
        {
            while (true)
            {
                var input = Console.ReadLine();

                if (int.TryParse(input, out int result))
                    return result;

                Console.WriteLine("Invalid ID. Please try again.");
            }
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
            var firstName = Prompt("Enter first name: ");
            var lastName = Prompt("Enter last name: ");

            bool ok = true;

            if (firstName.Length > 20 || lastName.Length > 20)
            {
                Console.WriteLine("Name is too long and will be truncated.");

                var continuePrompt = new EasyConsole.Menu()
                    .Add($"Continue", () => { })
                    .Add("Abort", () => ok = false);

                continuePrompt.Display();
            }

            if (!ok) return;

            try
            {
                Connection.AddUser(firstName, lastName);
                Console.WriteLine($"New user {firstName} {lastName} added succesfully.");
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

            if (!queryResult.Any())
            {
                Console.WriteLine("No users!");
                return;
            }

            var table = GetUserColumns();

            foreach (var person in queryResult)
            {
                table.AddRow(GetUserRow(person));
            }

            AnsiConsole.Render(table);
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

        private void AddTransaction()
        {
            Console.WriteLine("Provide the ID of the user to add a transaction for.");

            var person = GetPerson();
            if (person is InvalidPerson) return;

            Console.WriteLine("Provide transaction amount.");

            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                var trans = new Transaction(amount);

                Connection.AddTransaction(person.Id, new Transaction(amount));
                Console.WriteLine($"Transaction for {trans.Amount} added.");
                return;
            }

            Console.WriteLine("Number formatted incorrectly - please try again.");
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
