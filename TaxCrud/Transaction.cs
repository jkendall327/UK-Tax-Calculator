using System;
using System.Runtime.Serialization;

namespace TaxCrud
{
    internal abstract record Transaction
    {
        public decimal Amount { get; set; }

        public DateTime Timestamp { get; set; }

        public static Transaction Create(decimal amount)
        {
            if (amount > 0)
            {
                return new Income(amount);
            }
            if (amount < 0)
            {
                return new Expenditure(amount);
            }

            return new InvalidTransaction();
        }
    }

    internal record Income : Transaction
    {
        public Income(decimal amount)
        {
            if (amount < 0) throw new InvalidTransactionException();

            Amount = amount;
        }
    }

    internal record Expenditure : Transaction
    {
        public Expenditure(decimal amount)
        {
            if (amount > 0) throw new InvalidTransactionException();

            Amount = amount;
        }
    }
    internal record InvalidTransaction : Transaction { }

    [Serializable]
    internal class InvalidTransactionException : Exception
    {
        private static readonly string DefaultMessage = "Transaction class was instantiate with invalid amounts.";

        public InvalidTransactionException() : base(DefaultMessage)
        {
        }

        public InvalidTransactionException(string message) : base(message)
        {
        }

        public InvalidTransactionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidTransactionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
