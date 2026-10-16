using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.Billing.ValueObjects
{
    // Bill-side credits only. Both kinds REDUCE what the guest owes. Money going back to the guest is a Refund
    // (its own value object), not an adjustment type: an adjustment has no payment method or reference, and a
    // type that quietly flips the sign of the balance would make the sign do double duty as a direction flag.

    // The answer to "how do I take a charge back without a negative Charge": the original Charge stays exactly
    // as posted (untouched, still visible in the audit trail) and this sits as a separate ledger line explaining
    // why the balance went down without the guest having paid anything. Reduces Balance the same way a Payment
    // does, but is never confused with one.
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
            // Amount is always positive - it represents a credit back to the
            // guest, same reasoning as why Charge forbids negative amounts:
            // the sign should never be doing double duty as a direction flag.
            if (amount.Amount <= 0)
                throw new DomainException("Adjustment amount must be positive - it represents a credit back to the guest.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("An adjustment requires a reason.");

            return new Adjustment(
                amount,
                type,
                reason.Trim(),
                DateTime.UtcNow);
        }

        public override string ToString() => $"{Type}: {Amount} - {Reason}";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Type;
            yield return Reason;
            yield return PostedAt;
        }
    }
}
