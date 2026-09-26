using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;

namespace Innova.Application.Billing.Folio.Commands.VoidFolio
{
    public sealed class VoidFolioCommandHandler( IBillingAssemblyService billingAssemblyService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<VoidFolioCommand>
    {
        public async Task<Unit> HandleAsync( VoidFolioCommand command, CancellationToken ct = default )
        {
            Domain.Billing.Aggregates.Folio folio = await billingAssemblyService.VoidAsync(command.FolioId, ct);

            await eventDispatcher.DispatchAsync(folio.DomainEvents, ct);
            folio.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
