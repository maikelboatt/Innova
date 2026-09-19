using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.Billing.ValueObjects
{
    public sealed class Charge:ValueObject
    {
        private Charge( Money amount,
                        string category,
                        string description,
                        DateTime postedAt )
        {
            Amount = amount;
            Category = category;
            Description = description;
            PostedAt = postedAt;

        }

        public Money Amount { get; }
        public string Category { get; }
        public string Description { get; }
        public DateTime PostedAt { get; }

        public static Charge Of( Money amount, string category, string description )
        {
            if (amount.Amount < 0)
                throw new DomainException("Charges cannot be negative — model refunds as a separate Adjustment.");

            if (string.IsNullOrWhiteSpace(category))
                throw new DomainException("Charge category is required");

            return new Charge(
                amount,
                category,
                description,
                DateTime.UtcNow);

        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Category;
            yield return Description;
            yield return PostedAt;
        }
    }
}
