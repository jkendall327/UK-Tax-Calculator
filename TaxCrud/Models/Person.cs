using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxCrud
{
    /// <summary>
    /// Represents a person with a list of financial transactions.
    /// </summary>
    internal record Person
    {
        internal int Id { get; init; }

        internal string FirstName { get; set; }
        internal string LastName { get; set; }
        internal string Name => FirstName + " " + LastName;

        internal decimal Balance => Transactions.Sum(x => x.Amount);

        internal List<Transaction> Transactions { get; set; } = new();

        public override string ToString() => $"[{Id}] {FirstName} {LastName}";

        internal decimal TaxOverLastYear() => Tax.Calculate(this, TimeSpan.FromDays(360), DateTime.Now);
    }

    /// <summary>
    /// Represents a <see cref="Person"/> object that is in an invalid state.
    /// </summary>
    internal record InvalidPerson : Person { }
}
