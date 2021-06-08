using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TaxCrud
{
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

        // https://stackoverflow.com/a/4630550
        public int GetHashCode([DisallowNull] Person obj) => (obj.Id, obj.Name, obj.Transactions).GetHashCode();
    }
}
