using Innova.Application.Billing.Folio.DTO;

namespace Innova.Presentation.Core.Billing.Folio.Details
{
    /// <summary>
    ///     One line of the folio statement: a charge, a payment or an adjustment. Charges and payments have no ids
    ///     in the domain (they are value objects), so entries are only ever listed, never addressed individually.
    /// </summary>
    public sealed class FolioLedgerEntryViewModel
    {
        private FolioLedgerEntryViewModel( string kindLabel,
                                           string kindBadge,
                                           string title,
                                           string? detail,
                                           decimal amount,
                                           string currency,
                                           bool isCredit,
                                           DateTime occurredAtUtc )
        {
            KindLabel = kindLabel;
            KindBadge = kindBadge;
            Title = title;
            Detail = detail ?? string.Empty;
            Amount = amount;
            IsCredit = isCredit;

            // A credit (payment or adjustment) reduces what is owed, so it is shown with a minus sign.
            AmountDisplay = isCredit
                                ? $"-{currency} {amount:N2}"
                                : $"{currency} {amount:N2}";

            // SQL Server returns DATETIME2 without a kind; the values are stored as UTC.
            OccurredAt = DateTime
                         .SpecifyKind(occurredAtUtc, DateTimeKind.Utc)
                         .ToLocalTime();
            OccurredDisplay = OccurredAt.ToString("dd MMM yyyy HH:mm");
        }

        public string KindLabel { get; }

        /// <summary>
        ///     "Neutral" (charge), "Success" (payment), "Info" (adjustment) or "Warning" (refund) - the view maps these to
        ///     brushes.
        /// </summary>
        public string KindBadge { get; }

        public string Title { get; }
        public string Detail { get; }
        public bool HasDetail => Detail.Length > 0;
        public decimal Amount { get; }
        public string AmountDisplay { get; }
        public bool IsCredit { get; }
        public DateTime OccurredAt { get; }
        public string OccurredDisplay { get; }

        public static FolioLedgerEntryViewModel FromCharge( ChargeDto dto ) => new(
            "Charge",
            "Neutral",
            dto.Category,
            dto.Description,
            dto.Amount,
            dto.Currency,
            false,
            dto.PostedAt);

        public static FolioLedgerEntryViewModel FromPayment( PaymentDto dto ) => new(
            "Payment",
            "Success",
            MethodLabel(dto.Method),
            dto.Reference,
            dto.Amount,
            dto.Currency,
            true,
            dto.ReceivedAt);

        /// <summary>Money going back to the guest: it raises the balance, so it is shown as a plain amount (not a credit).</summary>
        public static FolioLedgerEntryViewModel FromRefund( RefundDto dto ) => new(
            "Refund",
            "Warning",
            MethodLabel(dto.Method),
            string.IsNullOrWhiteSpace(dto.Reference)
                ? dto.Reason
                : $"{dto.Reason} · {dto.Reference}",
            dto.Amount,
            dto.Currency,
            false,
            dto.RefundedAt);

        public static FolioLedgerEntryViewModel FromAdjustment( AdjustmentDto dto ) => new(
            AdjustmentLabel(dto.Type),
            "Info",
            AdjustmentLabel(dto.Type),
            dto.Reason,
            dto.Amount,
            dto.Currency,
            true,
            dto.PostedAt);

        private static string MethodLabel( string method ) => method switch
                                                              {
                                                                  "BankTransfer" => "Bank transfer",
                                                                  "MobileMoney"  => "Mobile money",
                                                                  _              => method
                                                              };

        private static string AdjustmentLabel( string type ) => type switch
                                                                {
                                                                    "GoodwillCredit"    => "Goodwill credit",
                                                                    "BillingCorrection" => "Billing correction",
                                                                    _                   => type
                                                                };
    }
}
