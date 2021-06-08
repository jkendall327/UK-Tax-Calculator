﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    }

    /// <summary>
    /// Represents a <see cref="Person"/> object that is in an invalid state.
    /// </summary>
    internal record InvalidPerson : Person { }

    /// <summary>
    /// Custom equality comparer for <see cref="Person"/>. Needed because generic collections in record types are not compared by value but by reference.
    /// </summary>
    internal class PersonComparer : IEqualityComparer<Person>
    {
        public bool Equals(Person x, Person y)
        {
            if (x.Id != y.Id) return false;
            if (x.Name != y.Name) return false;

            foreach (var pair in x.Transactions.Zip(y.Transactions))
            {
                if (pair.First.Amount != pair.Second.Amount) return false;
                if (pair.First.Timestamp != pair.Second.Timestamp) return false;
            }

            return true;
        }

        public int GetHashCode([DisallowNull] Person obj) => obj.GetHashCode();
    }
}
