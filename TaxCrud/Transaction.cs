using System;

namespace TaxCrud
{
    internal record Transaction
    {
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }

        public Transaction(decimal amount)
        {
            Amount = Math.Round(amount, 2);
        }

        public Transaction()
        {
        }

        public override string ToString() => Timestamp + ": " + Amount;

    }
    internal record InvalidTransaction : Transaction
    {

    }
}
