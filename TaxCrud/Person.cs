using System.Collections.Generic;
using System.Linq;

namespace TaxCrud
{
    public record Person
    {
        public int Id { get; init; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name => FirstName + " " + LastName;

        public decimal Balance => Transactions.Sum(x => x.Amount);

        internal List<Transaction> Transactions { get; set; } = new();

        public override string ToString() => $"[{Id}] {FirstName} {LastName}";
    }

    public record InvalidPerson : Person { }
}
