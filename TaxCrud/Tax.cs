using System;
using System.Linq;

namespace TaxCrud
{
    /// <summary>
    /// Contains methods for calculating a <see cref="Person"/>'s tax.
    /// </summary>
    internal static class Tax
    {
        /// <summary>
        /// Calculates the tax a user in the UK should pay for income earned during a given timespan.
        /// </summary>
        /// <param name="range">Amount of time in which transactions should be considered.</param>
        /// <param name="endTime">Latest point at which a transaction should be considered.</param>
        /// <returns>The total tax to be paid.</returns>
        internal static decimal Calculate(Person person, TimeSpan range, DateTime endTime)
        {
            // https://www.gov.uk/income-tax-rates

            var transactionsInRange = person.Transactions
                .Where(x => x.Timestamp < endTime)
                .Where(x => x.Timestamp > (endTime - range));

            var totalIncome = transactionsInRange.Sum(x => x.Amount);

            if (totalIncome <= 0) return decimal.Zero;

            decimal personalAllowance = CalculatePersonalAllowance(totalIncome);

            var taxableIncome = totalIncome - personalAllowance;
            if (taxableIncome <= 0) return decimal.Zero;

            // get correct band of income via subtraction, then multiply by
            // appropriate rate to get the tax due
            var additionalRateTax = (taxableIncome - 150_000) * 0.45m;
            var higherRateTax = (taxableIncome - 50_271) * 0.40m;
            var basicRateTax = (taxableIncome - 12_571) * 0.20m;

            var totalTax = 0m;

            // ignore any negative tax!
            static decimal Clamped(decimal value) => Math.Clamp(value, 0, decimal.MaxValue);

            totalTax += Clamped(additionalRateTax);
            totalTax += Clamped(higherRateTax);
            totalTax += Clamped(basicRateTax);

            return totalTax;
        }

        private static decimal CalculatePersonalAllowance(decimal totalIncome)
        {
            // todo: this differs if you make over a certain amount or claim Marriage Allowance or Blind Person’s Allowance
            return 12570m;
        }
    }
}
