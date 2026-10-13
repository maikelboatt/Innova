namespace Innova.Infrastructure.Billing.Folio.Mapping
{
    public sealed class FolioOwnerOptionRow
    {
        public Guid OwnerId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string? Detail { get; set; }
        public int OpenFolioCount { get; set; }
    }
}
