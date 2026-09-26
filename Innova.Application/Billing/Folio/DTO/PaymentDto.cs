namespace Innova.Application.Billing.Folio.DTO
{
    public sealed record PaymentDto(
        decimal Amount,
        string Currency,
        string Method,
        string? Reference,
        DateTime ReceivedAt );
}
