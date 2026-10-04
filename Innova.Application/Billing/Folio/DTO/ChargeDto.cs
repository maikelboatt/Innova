namespace Innova.Application.Billing.Folio.DTO
{
    public sealed record ChargeDto(
        decimal Amount,
        string Currency,
        string Category,
        string? Description,
        DateTime PostedAt );
}
