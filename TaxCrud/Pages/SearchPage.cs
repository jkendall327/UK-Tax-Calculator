using EasyConsole;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TaxCrudTests")]

namespace TaxCrud
{
    /// <summary>
    /// Lets users search for profiles via name.
    /// </summary>
    internal class SearchPage : Page
    {
        readonly IDatabaseHelper Connection;

        public SearchPage(Program program, IDatabaseHelper connection) : base("Search users", program)
        {
            Connection = connection;
        }

        public override void Display()
        {
            base.Display();

            var fname = Input.ReadString("Input first name: ");
            var lname = Input.ReadString("Input last name: ");

            var results = Connection.GetByName(fname, lname);

            if (!results.Any())
            {
                Console.WriteLine("No users by that name found.");
                Console.ReadLine();
                Program.NavigateBack();
            }

            Console.WriteLine($"{results.Count()} users found.");

            foreach (var person in results)
            {
                Console.WriteLine(person.ToString());
            }
        }
    }
}
