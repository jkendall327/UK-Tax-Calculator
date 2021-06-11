using EasyConsole;
using Spectre.Console;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TaxCrudTests")]

namespace TaxCrud
{
    /// <summary>
    /// Houses basic CRUD actions relating to the <see cref="Person"/> and <see cref="Transaction"/> classes.
    /// </summary>
    internal class UserPage : Page
    {
        readonly IDatabaseHelper Connection;

        public Person Person { get; private set; }

        public UserPage(Program program, IDatabaseHelper connection) : base("User details", program)
        {
            Connection = connection;
        }

        public override void Display()
        {
            base.Display();

            Console.WriteLine("Enter user ID:");

            Person = Connection.GetPerson();
            if (Person is InvalidPerson) Program.NavigateHome();

            ViewUser();
        }

        private void ViewUser()
        {
            // update record
            Person = Connection.GetByID(Person.Id);

            Person.PrintToTable();

            // only print transactions table if there's some to print
            if (!Person.Transactions.IsEmpty()) PrintTransactions(5);

            new Menu()
                .Add($"Go back", () => Program.NavigateBack())
                .Add("Add transaction", AddTransaction)
                .Add("Edit user", Edit)
                .Add("Delete user", Delete)
            .Display();

            // loop back around, only break out when user chooses 'go back'
            ViewUser();
        }

        /// <summary>
        /// Encapsulates printing out a <see cref="Person"/>'s transactions.
        /// </summary>
        /// <param name="howMany">How many of the person's most recent transactions to print.</param>
        private void PrintTransactions(int howMany)
        {
            Console.WriteLine("Most recent transactions:");

            var transactions = new Table()
                .AddColumn("Timestamp")
                .AddColumn("Amount");

            foreach (var transaction in Person.Transactions.TakeLast(howMany))
            {
                transactions.AddRow(new string[] { transaction.Timestamp.ToString(), transaction.Amount.ToString() });
            }

            AnsiConsole.Render(transactions);
        }

        private void Edit()
        {
            Console.WriteLine($"Provide new name for {Person.Name}.");
            var fname = Input.ReadString("Input first name: ");
            var lname = Input.ReadString("Input last name: ");

            Console.WriteLine($"This change will affect user {Person.Name}. Continue?");

            new Menu()
                .Add($"Yes, update {Person.Name}'s name to {fname} {lname}", () =>
                {
                    Connection.UpdateName(Person.Id, fname, lname);
                    Console.WriteLine("User name updated.");
                })
                .Add("No, do not update name", () => Console.WriteLine("Aborting."))
            .Display();
        }

        private void Delete()
        {
            Console.WriteLine($"This will delete the user {Person.Name}. Continue?");

            new Menu()
                .Add($"Yes, delete {Person.Name} permanently", () =>
                {
                    Connection.DeleteUser(Person.Id);
                    Console.WriteLine("User deleted.");
                    Program.NavigateHome();
                })
                .Add("No, do not delete", () => Console.WriteLine("Aborting."))
            .Display();
        }

        private void AddTransaction()
        {
            Console.WriteLine("Provide transaction amount.");

            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                var trans = new Transaction(amount);

                Connection.AddTransaction(Person.Id, new Transaction(amount));
                Console.WriteLine($"Transaction for {trans.Amount} added.");
                return;
            }

            Console.WriteLine("Number formatted incorrectly - please try again.");
        }
    }
}
