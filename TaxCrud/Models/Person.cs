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

        internal decimal TaxOverLastYear() => CalculateTax(TimeSpan.FromDays(360), DateTime.Now);

        /// <summary>
        /// Calculates the tax a user in the UK should pay for income earned during a given timespan.
        /// </summary>
        /// <param name="range">Amount of time in which transactions should be considered.</param>
        /// <param name="endTime">Latest point at which a transaction should be considered.</param>
        /// <returns>The total tax to be paid.</returns>
        internal decimal CalculateTax(TimeSpan range, DateTime endTime)
        {
            // https://www.gov.uk/income-tax-rates

            var transactionsInRange = Transactions
                .Where(x => x.Timestamp < endTime)
                .Where(x => x.Timestamp > (endTime - range));

            var totalIncome = transactionsInRange.Sum(x => x.Amount);

            if (totalIncome <= 0) return decimal.Zero;

            decimal personalAllowance = CalculatePersonalAllowance(totalIncome);

            var taxableIncome = totalIncome - personalAllowance;
            if (taxableIncome <= 0) return decimal.Zero;

            // get correct band of income via subtraction, then multiply by
            // appropriate rate to get the tax due
            var additionalRateTax = (taxableIncome - 150000) * 0.45m;
            var higherRateTax = (taxableIncome - 50271) * 0.40m;
            var basicRateTax = (taxableIncome - 12571) * 0.20m;

            var totalTax = 0m;

            // ignore any negative tax!
            static decimal Clamped(decimal value) => Math.Clamp(value, 0, decimal.MaxValue);

            totalTax += Clamped(additionalRateTax);
            totalTax += Clamped(higherRateTax);
            totalTax += Clamped(basicRateTax);

            return totalTax;
        }

        private decimal CalculatePersonalAllowance(decimal totalIncome)
        {
            // todo: this differs if you make over a certain amount or claim Marriage Allowance or Blind Person’s Allowance
            return 12570m;
        }
    }

    /// <summary>
    /// Represents a <see cref="Person"/> object that is in an invalid state.
    /// </summary>
    internal record InvalidPerson : Person { }
}
