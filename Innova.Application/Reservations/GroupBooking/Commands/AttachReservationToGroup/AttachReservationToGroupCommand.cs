using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Reservations.GroupBooking.Commands.AttachReservationToGroup
{
    public record AttachReservationToGroupCommand( Guid GroupBookingId, Guid ReservationId ):ICommand<Unit>;
}
