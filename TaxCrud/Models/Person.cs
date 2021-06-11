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

        internal bool HasBlindPersonAllowance { get; set; } = false;
        internal bool HasMarriageAllowance { get; set; } = false;
        public bool IsMarriedOrInCivilPartnership { get; private set; }

        public decimal PersonalAllowance { get; private set; }
        public decimal Income { get; private set; }

        Person Partner { get; set; }
        List<Person> Dependents { get; set; } = new();

        TaxRate Rate { get; set; }

        internal bool CanClaimMarriageAllowance()
        {
            if (!IsMarriedOrInCivilPartnership) return false;
            if (Income > PersonalAllowance) return false;
            if (Partner.Rate != TaxRate.Basic) return false;

            return true;
        }

        public override string ToString() => $"[{Id}] {FirstName} {LastName}";

        internal decimal TaxOverLastYear() => Tax.Calculate(this, TimeSpan.FromDays(360), DateTime.Now);
    }

    internal enum TaxRate
    {
        None,
        Basic,
        Higher,
        Additional
    }

    /// <summary>
    /// Represents a <see cref="Person"/> object that is in an invalid state.
    /// </summary>
    internal record InvalidPerson : Person { }
}
