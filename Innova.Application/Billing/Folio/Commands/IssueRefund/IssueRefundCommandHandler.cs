using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.Shared.ValueObjects;
using FolioAggregate = Innova.Domain.Billing.Aggregates.Folio;

namespace Innova.Application.Billing.Folio.Commands.IssueRefund
{
    public sealed class IssueRefundCommandHandler( IBillingAssemblyService billingAssemblyService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<IssueRefundCommand>
    {
        public async Task<Unit> HandleAsync( IssueRefundCommand command, CancellationToken ct = default )
        {
            Money amount = Money.Of(command.Amount, command.Currency);

            FolioAggregate folio = await billingAssemblyService.IssueRefundAsync(
                                       command.FolioId,
                                       amount,
                                       command.Method,
                                       command.Reference,
                                       command.Reason,
                                       ct);

            await eventDispatcher.DispatchAsync(folio.DomainEvents, ct);
            folio.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
