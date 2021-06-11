using EasyConsole;
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
                queryResult.PrintToTable();
            }
            else
            {
                Console.WriteLine("No users!");
            }

            Console.WriteLine("Hit [Enter] to return.");
            Console.ReadLine();
            Program.NavigateHome();
        }
    }
}
