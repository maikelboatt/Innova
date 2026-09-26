using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Billing.Folio.Commands.PostAdjustment
{
    public sealed class PostAdjustmentCommandHandler( IBillingAssemblyService billingAssemblyService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<PostAdjustmentCommand>
    {
        public async Task<Unit> HandleAsync( PostAdjustmentCommand command, CancellationToken ct = default )
        {
            Money amount = Money.Of(command.Amount, command.Currency);

            Domain.Billing.Aggregates.Folio folio = await billingAssemblyService.PostAdjustmentAsync(
                                                        command.FolioId,
                                                        amount,
                                                        command.Type,
                                                        command.Reason,
                                                        ct);

            await eventDispatcher.DispatchAsync(folio.DomainEvents, ct);
            folio.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
