using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;

namespace Innova.Application.Billing.Folio.Commands.SettleFolio
{
    public sealed class SettleFolioCommandHandler( IBillingAssemblyService billingAssemblyService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<SettleFolioCommand>
    {
        public async Task<Unit> HandleAsync( SettleFolioCommand command, CancellationToken ct = default )
        {
            Domain.Billing.Aggregates.Folio folio = await billingAssemblyService.SettleAsync(command.FolioId, ct);

            await eventDispatcher.DispatchAsync(folio.DomainEvents, ct);
            folio.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
