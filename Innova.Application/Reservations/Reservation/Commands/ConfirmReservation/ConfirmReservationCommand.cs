using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Reservations.Reservation.Commands.ConfirmReservation
{
    public sealed record ConfirmReservationCommand( Guid ReservationId, bool HasGuarantee ):ICommand<Unit>;
}
