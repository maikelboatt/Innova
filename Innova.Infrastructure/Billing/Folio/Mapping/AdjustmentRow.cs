namespace Innova.Infrastructure.Billing.Folio.Mapping
{
    public class AdjustmentRow
    {
        public int Id { get; init; }
        public Guid FolioId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public string Reason { get; init; } = string.Empty;
        public DateTime PostedAt { get; init; }
    }
}
