namespace Innova.Infrastructure.Billing.Folio.Mapping
{
    public sealed class FolioSummaryRow
    {
        public Guid Id { get; init; }

        public string OwnerType { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;

        public decimal Balance { get; init; }

        public string Currency { get; init; } = string.Empty;
    }
}
