using Innova.Application.Abstractions.Messaging;
using Innova.Domain.Billing.ValueObjects;

namespace Innova.Application.Billing.Folio.Commands.IssueRefund
{
    /// <summary>Hands money back to the guest. Only possible while the folio holds a credit.</summary>
    public sealed record IssueRefundCommand(
        Guid FolioId,
        decimal Amount,
        string Currency,
        PaymentMethod Method,
        string? Reference,
        string Reason ):ICommand;
}
