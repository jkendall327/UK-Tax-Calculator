using EasyConsole;
using Spectre.Console;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TaxCrudTests")]

namespace TaxCrud
{
    /// <summary>
    /// Lists all current users in the database.
    /// </summary>
    internal class ViewUsersPage : Page
    {
        readonly IDatabaseHelper Connection;

        public ViewUsersPage(Program program, IDatabaseHelper connection) : base("View users", program)
        {
            Connection = connection;
        }

        public override void Display()
        {
            base.Display();

            var queryResult = Connection.GetAllUsers();

            if (queryResult.Any())
            {
                var table = Person.GetEmptyTable();

                foreach (var person in queryResult)
                {
                    table.AddRow(person.TableData);
                }

                AnsiConsole.Render(table);
            }
            else
            {
                Console.WriteLine("No users!");
            }

            Console.ReadLine();

            Program.NavigateHome();
        }
    }
}
