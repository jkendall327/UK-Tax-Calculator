using EasyConsole;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TaxCrudTests")]

namespace TaxCrud
{
    /// <summary>
    /// Resets database, destroying all <see cref="Person"/> and <see cref="Transaction"/> objects.
    /// </summary>
    internal class ClearDatabasePage : Page
    {
        readonly IDatabaseHelper Connection;

        public ClearDatabasePage(Program program, IDatabaseHelper connection) : base("Clear database", program)
        {
            Connection = connection;
        }

        public override void Display()
        {
            base.Display();

            Console.WriteLine("This will wipe all data, including users and financial transactions. Are you sure?");

            var yesOrNo = new Menu()
                .Add("Yes, delete everything", () =>
                {
                    Connection.ResetDatabase();
                    Console.WriteLine("Database reset.");
                })
                .Add("No, keep my data", () => { Console.WriteLine("Aborting."); });

            yesOrNo.Display();

            Console.ReadLine();

            Program.NavigateBack();
        }
    }
}
