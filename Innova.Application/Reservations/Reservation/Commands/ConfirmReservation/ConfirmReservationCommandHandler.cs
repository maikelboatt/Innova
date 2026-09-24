using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.Reservations.Reservation.Commands.ConfirmReservation
{
    public class ConfirmReservationCommandHandler( IReservationService reservationService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<ConfirmReservationCommand, Unit>
    {
        public async Task<Unit> HandleAsync( ConfirmReservationCommand command, CancellationToken ct = default )
        {
            ReservationId reservationId = ReservationId.From(command.ReservationId);

            Domain.Reservations.Aggregates.Reservation reservation = await reservationService.ConfirmAsync(reservationId, command.HasGuarantee, ct);

            await eventDispatcher.DispatchAsync(reservation.DomainEvents, ct);
            reservation.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
