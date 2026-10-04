namespace Innova.Infrastructure.Billing.Folio.Mapping
{
    public class PaymentRow
    {
        public int Id { get; init; }
        public Guid FolioId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = string.Empty;
        public string Method { get; init; } = string.Empty;
        public string? Reference { get; init; }
        public DateTime ReceivedAt { get; init; }
    }
}
