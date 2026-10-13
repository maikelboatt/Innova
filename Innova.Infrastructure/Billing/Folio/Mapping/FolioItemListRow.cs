namespace Innova.Infrastructure.Billing.Folio.Mapping
{
    public sealed class FolioListItemRow
    {
        public Guid FolioId { get; set; }
        public string OwnerType { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string? OwnerDetail { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalCharges { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal TotalAdjustments { get; set; }
        public decimal Balance { get; set; }
        public DateTime? LastActivityAt { get; set; }
    }
}
