using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.Reservations.Reservation.Commands.MarkNoShow
{
    public sealed class MarkReservationNoShowCommandHandler( IReservationService reservationService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<MarkReservationNoShowCommand, Unit>
    {
        public async Task<Unit> HandleAsync( MarkReservationNoShowCommand command, CancellationToken ct = default )
        {
            ReservationId reservationId = ReservationId.From(command.ReservationId);

            Domain.Reservations.Aggregates.Reservation reservation = await reservationService.MarkNoShowAsync(reservationId, ct);

            await eventDispatcher.DispatchAsync(reservation.DomainEvents, ct);
            reservation.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
