using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.Billing.ValueObjects
{
    // Money going BACK to the guest: the mirror image of a Payment.
    //
    // A refund is not an Adjustment. An adjustment changes what the guest owes (a bill-side credit) and has no
    // method or reference; a refund is a cash movement (the till, the card terminal, the bank) and needs both.
    // Keeping them apart means every amount in the ledger has an unambiguous direction:
    //   bill side:  Charge (+)   Adjustment (-)
    //   cash side:  Payment (-)  Refund (+)       Balance = Charges - Adjustments - Payments + Refunds
    public sealed class Refund:ValueObject
    {
        private Refund( Money amount,
                        PaymentMethod method,
                        string? reference,
                        string reason,
                        DateTime refundedAt )
        {
            Amount = amount;
            Method = method;
            Reference = reference;
            Reason = reason;
            RefundedAt = refundedAt;
        }

        public Money Amount { get; }
        public PaymentMethod Method { get; }
        public string? Reference { get; }
        public string Reason { get; }
        public DateTime RefundedAt { get; }

        public static Refund Of( Money amount,
                                 PaymentMethod method,
                                 string? reference,
                                 string reason )
        {
            // Always positive: the direction is the type itself, never the sign (same rule as Charge and Payment).
            if (amount.Amount <= 0)
                throw new DomainException("Refund amount must be positive.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("A refund requires a reason.");

            return new Refund(
                amount,
                method,
                string.IsNullOrWhiteSpace(reference)
                    ? null
                    : reference.Trim(),
                reason.Trim(),
                DateTime.UtcNow);
        }

        // For FolioRepository only: rebuilds a refund with its ORIGINAL timestamp. Mirror whatever your
        // Payment/Charge value objects use for this, so every ledger line round-trips the same way.
        public static Refund Reconstitute( Money amount,
                                           PaymentMethod method,
                                           string? reference,
                                           string reason,
                                           DateTime refundedAt ) => new(
            amount,
            method,
            reference,
            reason,
            refundedAt);

        public override string ToString() => $"Refund ({Method}): {Amount} - {Reason}";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Method;
            yield return Reference;
            yield return Reason;
            yield return RefundedAt;
        }
    }
}
