namespace Innova.Infrastructure.Billing.Folio.Mapping
{
    public sealed class RefundRow
    {
        public Guid FolioId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime RefundedAt { get; set; }
    }
}
