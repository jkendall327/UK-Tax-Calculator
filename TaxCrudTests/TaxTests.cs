using FluentAssertions;
using System;
using TaxCrud;
using Xunit;

namespace TaxCrudTests
{
    public class TaxTests
    {
        [Theory]
        [InlineData(360, "0.00")]
        public void CalculateTax_ReturnsCorrectValue_WhenInputIsWellFormed(int days, string expectedTax)
        {
            // arrange
            var range = TimeSpan.FromDays(days);
            var expected = decimal.Parse(expectedTax);

            var _sut = new Person
            {
                Transactions = new()
                {
                    new Transaction() { Amount = 16.22m, Timestamp = DateTime.Today }
                }
            };

            // act
            decimal taxToPay = Tax.Calculate(_sut, range, DateTime.Now);

            // assert
            expected.Should().Be(taxToPay);
        }


        [Fact]
        public void CalculateTax_ReturnsZero_WhenIncomeIsNegative()
        {
            // arrange
            var _sut = new Person
            {
                Transactions = new()
                {
                    new Transaction() { Amount = -16.22m, Timestamp = DateTime.Today },
                    new Transaction() { Amount = -1326.22m, Timestamp = DateTime.Today },
                    new Transaction() { Amount = -126.22m, Timestamp = DateTime.Today },
                    new Transaction() { Amount = -163946.22m, Timestamp = DateTime.Today }
                }
            };

            // act
            decimal taxToPay = Tax.Calculate(_sut, TimeSpan.FromDays(360), DateTime.Now);

            // assert
            taxToPay.Should().Be(0, "because total income was negative");
        }


        [Fact]
        public void CalculateTax_SHouldHaveHigherPersonalAllowance_WhenPersonIsBlind()
        {
            // arrange
            var sut = new Person() { HasBlindPersonAllowance = true };
            sut.Transactions.Add(new Transaction() { Amount = 400_000_000, Timestamp = DateTime.Today });

            var comparison = new Person() { HasBlindPersonAllowance = false };
            comparison.Transactions.Add(new Transaction() { Amount = 400_000_000, Timestamp = DateTime.Today });

            // act
            var withoutAllowance = Tax.Calculate(comparison, TimeSpan.FromDays(360), DateTime.Now);
            var withAllowance = Tax.Calculate(sut, TimeSpan.FromDays(360), DateTime.Now);

            // assert
            withAllowance.Should().BeLessThan(withoutAllowance);
        }

        [Fact]
        public void CalculateTax_SHouldHaveHigherPersonalAllowance_WhenPersonHasMarriageAllowance()
        {
            // arrange
            var sut = new Person() { HasMarriageAllowance = true };
            sut.Transactions.Add(new Transaction() { Amount = 400_000_000, Timestamp = DateTime.Today });

            var comparison = new Person() { HasMarriageAllowance = false };
            comparison.Transactions.Add(new Transaction() { Amount = 400_000_000, Timestamp = DateTime.Today });

            // act
            var withoutAllowance = Tax.Calculate(comparison, TimeSpan.FromDays(360), DateTime.Now);
            var withAllowance = Tax.Calculate(sut, TimeSpan.FromDays(360), DateTime.Now);

            // assert
            withAllowance.Should().BeLessThan(withoutAllowance);

        }
    }
}
