namespace Innova.Application.Billing.Folio.DTO
{
    public sealed record FolioSummaryDto(
        Guid FolioId,
        string OwnerType,
        string Status,
        decimal Balance,
        string Currency );
}
