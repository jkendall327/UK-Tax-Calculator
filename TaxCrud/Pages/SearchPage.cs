using EasyConsole;
using Spectre.Console;
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

            var results = Connection.GetByName(fname, lname).ToList();

            if (!results.Any())
            {
                Console.WriteLine("No users by that name found.");
                Console.ReadLine();
                Program.NavigateBack();
            }

            Console.WriteLine($"{results.Count()} potential results found.");

            var table = Person.GetEmptyTable();

            foreach (var person in results)
            {
                table.AddRow(person.TableData);
            }

            AnsiConsole.Render(table);

            Console.WriteLine("Hit [Enter] to return.");
            Console.ReadLine();
            Program.NavigateBack();
        }
    }
}
