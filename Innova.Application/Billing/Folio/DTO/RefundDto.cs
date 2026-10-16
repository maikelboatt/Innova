namespace Innova.Application.Billing.Folio.DTO
{
    public sealed record RefundDto(
        decimal Amount,
        string Currency,
        string Method,
        string? Reference,
        string Reason,
        DateTime RefundedAt );
}
