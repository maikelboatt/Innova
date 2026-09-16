using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.Shared.ValueObjects
{
    public sealed class Money:ValueObject
    {
        private Money( decimal amount, string currency )
        {
            Amount = amount;
            Currency = currency;
        }

        public decimal Amount { get; }
        public string Currency { get; }

        public static Money Of( decimal amount, string currency = "GHS" )
        {
            if (amount < 0) throw new DomainException("Money amount cannot be negative.");
            if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Currency is required.");
            return new Money(amount, currency);
        }

        public static Money Zero( string currency = "GHS" ) => new(0, currency);

        public Money Add( Money other )
        {
            if (other.Currency != Currency) throw new DomainException("Cannot combine different currencies.");
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract( Money other )
        {
            if (other.Currency != Currency) throw new DomainException("Cannot combine different currencies.");
            return new Money(Amount - other.Amount, Currency);
        }

        public override string ToString() => $"Amount: '{Amount}' Currency: '{Currency}'";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
