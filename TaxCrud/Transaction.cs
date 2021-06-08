using System;

namespace TaxCrud
{
    internal record Transaction
    {
        public decimal Amount { get; set; }

        public DateTime Timestamp { get; set; }
    }
    internal record InvalidTransaction : Transaction { }
}
