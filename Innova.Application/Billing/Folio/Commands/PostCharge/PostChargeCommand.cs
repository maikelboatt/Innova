using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Billing.Folio.Commands.PostCharge
{
    public sealed record PostChargeCommand(
        Guid FolioId,
        decimal Amount,
        string Currency,
        string Category,
        string Description ):ICommand<Unit>;
}
