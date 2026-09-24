using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.Reservations.Reservation.Commands.CancelReservation
{
    public sealed class CancelReservationCommandHandler( IReservationService reservationService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<CancelReservationCommand, Unit>
    {
        public async Task<Unit> HandleAsync( CancelReservationCommand command, CancellationToken ct = default )
        {
            ReservationId reservationId = ReservationId.From(command.ReservationId);

            Domain.Reservations.Aggregates.Reservation reservation = await reservationService.CancelAsync(reservationId, ct);

            await eventDispatcher.DispatchAsync(reservation.DomainEvents, ct);
            reservation.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
