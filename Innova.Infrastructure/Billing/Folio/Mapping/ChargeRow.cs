namespace Innova.Infrastructure.Billing.Folio.Mapping
{
    public class ChargeRow
    {
        public int Id { get; init; }
        public Guid FolioId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public string? Description { get; init; }
        public DateTime PostedAt { get; init; }
    }
}
