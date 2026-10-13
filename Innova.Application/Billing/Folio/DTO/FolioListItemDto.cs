namespace Innova.Application.Billing.Folio.DTO
{
    /// <summary>
    ///     One row of the folio list. The owner is already resolved to a display name (a folio only stores
    ///     OwnerType + OwnerId) and the totals are computed in SQL, so the list never loads charge collections.
    ///     Balance = TotalCharges - TotalPayments - TotalAdjustments, the same formula as Folio.Balance.
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
        decimal TotalAdjustments,
        decimal Balance,
        DateTime? LastActivityAt );
}
