using System.Collections.Generic;
using System.Linq;

namespace TaxCrud
{
    internal record Person
    {
        internal int Id { get; init; }

        internal string FirstName { get; set; }
        internal string LastName { get; set; }
        internal string Name => FirstName + " " + LastName;

        internal decimal Balance => Transactions.Sum(x => x.Amount);

        internal List<Transaction> Transactions { get; set; } = new();

        public override string ToString() => $"[{Id}] {FirstName} {LastName}";
    }

    internal record InvalidPerson : Person { }
}
