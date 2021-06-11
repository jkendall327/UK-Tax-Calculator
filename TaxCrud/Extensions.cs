using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxCrud
{
    internal static class Extensions
    {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
            => source.Skip(Math.Max(0, source.Count() - n));

        public static bool IsEmpty<T>(this IEnumerable<T> source) => !source.Any();

        public static string Truncate(this string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Format a <see cref="decimal"/> as a monetary value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The formatted string.</returns>
        public static string ToMoney(this decimal value) => value.ToString("#,##0.00");

        /// <summary>
        /// Format a <see cref="Person"/> object into an <see cref="AnsiConsole"/> table.
        /// </summary>
        /// <param name="person">The person to list in a table.</param>
        public static void PrintToTable(this Person person)
        {
            PrintToTable(new List<Person>() { person });
        }

        /// <summary>
        /// Format an enumerable of <see cref="Person"/> objects to an <see cref="AnsiConsole"/> table.
        /// </summary>
        /// <param name="people">The enumerable of people to format.</param>
        public static void PrintToTable(this IEnumerable<Person> people)
        {
            var table = new Table()
                .AddColumn("Id")
                .AddColumn("Name")
                .AddColumn("Current balance")
                .AddColumn("Outstanding tax (past year)");

            foreach (var person in people)
            {
                table.AddRow(new string[] {
                    person.Id.ToString(),
                    person.Name,
                    person.Balance.ToMoney(),
                    person.TaxOverLastYear().ToMoney() });
            }

            AnsiConsole.Render(table);
        }
    }
}
