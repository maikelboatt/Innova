using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Services;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.Reservations.GroupBooking.Commands.DetachReservationFromGroup
{
    public class DetachReservationFromGroupCommandHandler( GroupBookingService groupBookingService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<DetachReservationFromGroupCommand, Unit>
    {
        public async Task<Unit> HandleAsync( DetachReservationFromGroupCommand command, CancellationToken ct = default )
        {
            GroupBookingId bookingId = GroupBookingId.From(command.GroupBookingId);
            ReservationId reservationId = ReservationId.From(command.ReservationId);

            Domain.Reservations.Aggregates.GroupBooking booking = await groupBookingService.DetachReservationAsync(
                                                                      bookingId,
                                                                      reservationId,
                                                                      ct);

            await eventDispatcher.DispatchAsync(booking.DomainEvents, ct);

            booking.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
