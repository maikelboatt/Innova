using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Reservations.GroupBooking.Commands.DetachReservationFromGroup
{
    public sealed record DetachReservationFromGroupCommand( Guid GroupBookingId, Guid ReservationId ):ICommand<Unit>;
}
