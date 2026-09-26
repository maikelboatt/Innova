using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Application.FrontDesk.Stay.Commands.CheckIn;
using Innova.Domain.FrontDesk.ValueObjects;

namespace Innova.Application.FrontDesk.Stay.Commands.CheckOut
{
    public sealed class CheckOutCommandHandler( IStayService stayService, IDomainEventDispatcher eventDispatcher ):ICommandHandler<CheckInCommand, Guid>
    {
        public async Task<Guid> HandleAsync( CheckInCommand command, CancellationToken ct = default )
        {
            StayId stayId = StayId.From(command.ReservationId);

            Domain.FrontDesk.Aggregates.Stay stay = await stayService.CheckOutAsync(stayId, ct);

            await eventDispatcher.DispatchAsync(stay.DomainEvents, ct);
            stay.ClearDomainEvents();

            return stay.Id.Value;
        }
    }
}
