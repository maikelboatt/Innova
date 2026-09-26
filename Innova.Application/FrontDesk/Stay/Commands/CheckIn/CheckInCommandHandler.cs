using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.FrontDesk.Stay.Commands.CheckIn
{
    public sealed class CheckInCommandHandler( IStayService stayService, IDomainEventDispatcher eventDispatcher ):ICommandHandler<CheckInCommand, Guid>
    {
        public async Task<Guid> HandleAsync( CheckInCommand command, CancellationToken ct = default )
        {
            ReservationId reservationId = ReservationId.From(command.ReservationId);
            GuestId guestId = GuestId.From(command.PrimaryOccupantId);

            Domain.FrontDesk.Aggregates.Stay stay = await stayService.CheckInAsync(reservationId, guestId, ct);

            await eventDispatcher.DispatchAsync(stay.DomainEvents, ct);
            stay.ClearDomainEvents();

            return stay.Id.Value;
        }
    }
}
