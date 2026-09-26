namespace Innova.Application.Billing.Folio.DTO
{
    public sealed record AdjustmentDto(
        decimal Amount,
        string Currency,
        string Type,
        string Reason,
        DateTime PostedAt );
}
