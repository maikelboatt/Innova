using Innova.Application.Billing.Folio.DTO;

namespace Innova.Presentation.Core.Billing.Folio.Lists
{
    /// <summary>
    ///     Lightweight display model for a single row of the folio list.
    ///     No commands, IoC dependencies or asynchronous work.
    /// </summary>
    public sealed class FolioListItemViewModel
    {
        public FolioListItemViewModel( FolioListItemDto dto )
        {
            FolioId = dto.FolioId;

            OwnerType = dto.OwnerType;
            OwnerTypeLabel = GetOwnerTypeLabel(dto.OwnerType);
            OwnerName = dto.OwnerName;
            OwnerDetail = string.IsNullOrWhiteSpace(dto.OwnerDetail)
                              ? "-"
                              : dto.OwnerDetail;

            Currency = dto.Currency;
            Status = dto.Status;
            StatusLabel = dto.Status;
            StatusBadge = GetStatusBadge(dto.Status);

            TotalCharges = dto.TotalCharges;

            // Payments and adjustments (goodwill credits, corrections) both reduce what is owed,
            // so the list shows them together: Charges - Paid/credited = Balance.
            TotalPaid = dto.TotalPayments + dto.TotalAdjustments;
            Balance = dto.Balance;

            ChargesDisplay = FormatMoney(dto.Currency, dto.TotalCharges);
            PaidDisplay = FormatMoney(dto.Currency, TotalPaid);
            BalanceDisplay = FormatMoney(dto.Currency, dto.Balance);
            BalanceBadge = GetBalanceBadge(dto.Status, dto.Balance);

            // SQL Server returns DATETIME2 values without a kind; they are stored as UTC.
            LastActivityAt = dto.LastActivityAt is { } utc
                                 ? DateTime
                                   .SpecifyKind(utc, DateTimeKind.Utc)
                                   .ToLocalTime()
                                 : null;
            LastActivityDisplay = LastActivityAt?.ToString("dd MMM yyyy HH:mm") ?? "-";
        }

        public Guid FolioId { get; }

        // ── Owner ────────────────────────────────────────────────
        public string OwnerType { get; }
        public string OwnerTypeLabel { get; }
        public string OwnerName { get; }
        public string OwnerDetail { get; }

        // ── Status ───────────────────────────────────────────────
        public string Currency { get; }
        public string Status { get; }
        public string StatusLabel { get; }

        /// <summary>"Info" (open), "Success" (settled) or "None" (void) - the view maps these to brushes.</summary>
        public string StatusBadge { get; }

        // ── Money (raw values are kept for sorting) ──────────────
        public decimal TotalCharges { get; }
        public decimal TotalPaid { get; }
        public decimal Balance { get; }
        public string ChargesDisplay { get; }
        public string PaidDisplay { get; }
        public string BalanceDisplay { get; }

        /// <summary>"Danger" = guest owes money, "Warning" = overpaid (credit), "Success" = nothing owed, "None" = void.</summary>
        public string BalanceBadge { get; }

        public bool HasOutstanding => Status == "Open" && Balance > 0;

        // ── Activity ─────────────────────────────────────────────
        public DateTime? LastActivityAt { get; }
        public string LastActivityDisplay { get; }

        // ── Helpers ──────────────────────────────────────────────
        private static string GetOwnerTypeLabel( string ownerType )
        {
            return ownerType switch
                   {
                       "Stay"            => "Room stay",
                       "GuestWithinStay" => "Guest in stay",
                       "GroupBooking"    => "Group booking",
                       "Guest"           => "Guest",
                       _                 => ownerType
                   };
        }

        private static string GetStatusBadge( string status )
        {
            return status switch
                   {
                       "Open"    => "Info",
                       "Settled" => "Success",
                       _         => "None"
                   };
        }

        private static string GetBalanceBadge( string status, decimal balance )
        {
            if (status == "Void") return "None";

            return balance switch
                   {
                       > 0 => "Danger",
                       < 0 => "Warning",
                       _   => "Success"
                   };
        }

        private static string FormatMoney( string currency, decimal amount ) => amount < 0
                                                                                    ? $"-{currency} {Math.Abs(amount):N2}"
                                                                                    : $"{currency} {amount:N2}";
    }
}
