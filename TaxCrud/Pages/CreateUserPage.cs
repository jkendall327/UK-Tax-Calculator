using EasyConsole;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TaxCrudTests")]

namespace TaxCrud
{
    /// <summary>
    /// Lets users create a new <see cref="Person"/> entry.
    /// </summary>
    internal class CreateUserPage : Page
    {
        readonly IDatabaseHelper Connection;

        public CreateUserPage(Program program, IDatabaseHelper connection) : base("Create user", program)
        {
            Connection = connection;
        }

        public override void Display()
        {
            base.Display();

            var firstName = Input.ReadString("Enter first name: ");
            var lastName = Input.ReadString("Enter last name: ");

            if (firstName.Length > 20 || lastName.Length > 20)
            {
                Console.WriteLine("Name is too long and will be truncated.");
                Console.WriteLine("Preview: " + firstName.Truncate(20) + " " + lastName.Truncate(20));

                var continuePrompt = new Menu()
                    .Add($"Continue", () =>
                    {
                        Connection.AddUser(firstName, lastName);
                        Console.WriteLine($"New user {firstName} {lastName} added succesfully.");
                    })
                    .Add("Abort", () => Console.WriteLine("Aborting..."));

                continuePrompt.Display();
            }
            else
            {
                Connection.AddUser(firstName, lastName);
                Console.WriteLine($"New user {firstName} {lastName} added succesfully.");
            }

            Console.ReadLine();
            Program.NavigateTo<ViewUsersPage>();
        }
    }
}
