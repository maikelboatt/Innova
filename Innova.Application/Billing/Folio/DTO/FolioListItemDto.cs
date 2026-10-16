namespace Innova.Application.Billing.Folio.DTO
{
    /// <summary>
    ///     One row of the folio list. The owner is already resolved to a display name (a folio only stores
    ///     OwnerType + OwnerId) and the totals come from the billing.FolioBalance view, so the list never loads
    ///     entry collections and the balance formula lives in exactly one place on the read side.
    ///     Balance = TotalCharges - TotalAdjustments - TotalPayments + TotalRefunds.
    /// </summary>
    public sealed record FolioListItemDto(
        Guid FolioId,
        string OwnerType,
        Guid OwnerId,
        string OwnerName,
        string? OwnerDetail,
        string Currency,
        string Status,
        decimal TotalCharges,
        decimal TotalPayments,
        decimal TotalRefunds,
        decimal TotalAdjustments,
        decimal Balance,
        DateTime? LastActivityAt );
}
