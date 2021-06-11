using EasyConsole;
using Humanizer;
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

            var resultWord = "user"
                .ToQuantity(queryResult.Count(), ShowQuantityAs.Words)
                .Transform(To.SentenceCase);

            Console.WriteLine($"{resultWord} found.");

            if (queryResult.Any())
            {
                queryResult.PrintToTable();
            }

            Console.WriteLine("Hit [Enter] to return.");
            Console.ReadLine();
            Program.NavigateHome();
        }
    }
}
