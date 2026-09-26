using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Billing.Folio.Commands.PostCharge
{
    public sealed class PostChargeCommandHandler( IBillingAssemblyService billingAssemblyService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<PostChargeCommand>
    {
        public async Task<Unit> HandleAsync( PostChargeCommand command, CancellationToken ct = default )
        {
            Money amount = Money.Of(command.Amount, command.Currency);

            Domain.Billing.Aggregates.Folio folio = await billingAssemblyService.PostChargeAsync(
                                                        command.FolioId,
                                                        amount,
                                                        command.Category,
                                                        command.Description,
                                                        ct);

            await eventDispatcher.DispatchAsync(folio.DomainEvents, ct);
            folio.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
