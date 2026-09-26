using Innova.Application.Abstractions.Messaging;
using Innova.Domain.Billing.ValueObjects;

namespace Innova.Application.Billing.Folio.Commands.RecordPayment
{
    public sealed record RecordPaymentCommand(
        Guid FolioId,
        decimal Amount,
        string Currency,
        PaymentMethod Method,
        string? Reference ):ICommand<Unit>;
}
