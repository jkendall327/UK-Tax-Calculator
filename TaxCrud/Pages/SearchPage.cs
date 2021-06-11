using EasyConsole;
using Humanizer;
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

            var resultWord = "potential match"
                .ToQuantity(results.Count, ShowQuantityAs.Words)
                .Transform(To.SentenceCase);

            Console.WriteLine($"{resultWord} found.");

            if (results.Any())
            {
                results.PrintToTable();
            }

            Console.WriteLine("Hit [Enter] to return.");
            Console.ReadLine();
            Program.NavigateBack();
        }
    }
}
