using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.Billing.ValueObjects
{
    public sealed class Payment:ValueObject
    {
        private Payment( Money amount,
                         PaymentMethod paymentMethod,
                         DateTime receivedAt,
                         string? reference )
        {
            Amount = amount;
            PaymentMethod = paymentMethod;
            ReceivedAt = receivedAt;
            Reference = reference?.Trim();
        }

        public Money Amount { get; }
        public DateTime ReceivedAt { get; }
        public PaymentMethod PaymentMethod { get; }
        public string? Reference { get; }

        public static Payment Of( Money amount,
                                  PaymentMethod paymentMethod,
                                  string? reference = null )
        {
            if (amount.Amount <= 0)
                throw new DomainException("Money must be positive.");

            return new Payment(
                amount,
                paymentMethod,
                DateTime.UtcNow,
                reference);
        }

        public override string ToString() =>
            $"Amount: '{Amount.Amount}' of currency '{Amount.Currency}' was paid at '{ReceivedAt}' with payment method '{PaymentMethod.ToString()}'";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return ReceivedAt;
            yield return PaymentMethod;
            yield return Reference;
        }
    }
}
