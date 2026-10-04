namespace Innova.Infrastructure.Billing.Folio.Mapping
{
    public class FolioRow
    {
        public Guid Id { get; init; }
        public string OwnerType { get; init; } = string.Empty;
        public Guid OwnerId { get; init; }
        public string Currency { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public decimal Balance { get; init; }
    }
}
