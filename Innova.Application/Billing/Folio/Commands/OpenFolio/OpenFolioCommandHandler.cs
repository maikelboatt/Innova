using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.Billing.ValueObjects;

namespace Innova.Application.Billing.Folio.Commands.OpenFolio
{
    // Assumes FolioOwner.Of(FolioOwnerType, Guid) was added, per the
    // centralizing-factory suggestion — collapses the OwnerType/OwnerId
    // reconstruction to one line instead of a switch in the handler.
    public sealed class OpenFolioCommandHandler( IBillingAssemblyService billingAssemblyService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<OpenFolioCommand, Guid>
    {
        public async Task<Guid> HandleAsync( OpenFolioCommand command, CancellationToken ct = default )
        {
            FolioOwner owner = FolioOwner.Of(command.OwnerType, command.OwnerId);

            Domain.Billing.Aggregates.Folio folio = await billingAssemblyService.OpenFolioAsync(owner, command.Currency, ct);

            await eventDispatcher.DispatchAsync(folio.DomainEvents, ct);
            folio.ClearDomainEvents();

            return folio.Id.Value;
        }
    }
}
