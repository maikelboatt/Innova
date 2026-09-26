using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Billing.Folio.Commands.RecordPayment
{
    public sealed class RecordPaymentCommandHandler( IBillingAssemblyService billingAssemblyService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<RecordPaymentCommand>
    {
        public async Task<Unit> HandleAsync( RecordPaymentCommand command, CancellationToken ct = default )
        {
            Money amount = Money.Of(command.Amount, command.Currency);

            Domain.Billing.Aggregates.Folio folio = await billingAssemblyService.PostPaymentAsync(
                                                        command.FolioId,
                                                        amount,
                                                        command.Method,
                                                        command.Reference,
                                                        ct);

            await eventDispatcher.DispatchAsync(folio.DomainEvents, ct);
            folio.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
