using Innova.Application.Abstractions.Messaging;
using Innova.Domain.Billing.ValueObjects;

namespace Innova.Application.Billing.Folio.Commands.PostAdjustment
{
    public sealed record PostAdjustmentCommand(
        Guid FolioId,
        decimal Amount,
        string Currency,
        AdjustmentType Type,
        string Reason ):ICommand<Unit>;
}
