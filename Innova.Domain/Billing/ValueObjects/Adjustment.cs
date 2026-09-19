using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.Billing.ValueObjects
{
    public sealed class Adjustment:ValueObject
    {
        private Adjustment( Money amount,
                            AdjustmentType type,
                            string reason,
                            DateTime postedAt )
        {
            Amount = amount;
            Type = type;
            Reason = reason;
            PostedAt = postedAt;
        }

        public Money Amount { get; }
        public AdjustmentType Type { get; }
        public string Reason { get; }
        public DateTime PostedAt { get; }

        public static Adjustment Of( Money amount, AdjustmentType type, string reason )
        {
            // Amount is always positive — it represents a credit back to the
            // guest, same reasoning as why Charge forbids negative amounts:
            // the sign should never be doing double duty as a direction flag.
            if (amount.Amount <= 0)
                throw new DomainException("Adjustment amount must be positive — it represents a credit back to the guest.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("An adjustment requires a reason.");

            return new Adjustment(
                amount,
                type,
                reason.Trim(),
                DateTime.UtcNow);
        }

        public override string ToString() => $"{Type}: {Amount} — {Reason}";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Type;
            yield return Reason;
            yield return PostedAt;
        }
    }
}
