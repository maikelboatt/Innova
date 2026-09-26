namespace Innova.Application.Billing.Folio.DTO
{
    public sealed record FolioDto(
        Guid FolioId,
        string OwnerType,
        Guid OwnerId,
        string Currency,
        string Status,
        decimal Balance,
        IReadOnlyCollection<ChargeDto> Charges,
        IReadOnlyCollection<PaymentDto> Payments,
        IReadOnlyCollection<AdjustmentDto> Adjustments );
}
