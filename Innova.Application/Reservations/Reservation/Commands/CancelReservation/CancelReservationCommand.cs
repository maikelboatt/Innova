using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Reservations.Reservation.Commands.CancelReservation
{
    public sealed record CancelReservationCommand( Guid ReservationId ):ICommand<Unit>;
}
