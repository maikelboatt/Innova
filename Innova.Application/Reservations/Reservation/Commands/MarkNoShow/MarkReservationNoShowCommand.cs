using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Reservations.Reservation.Commands.MarkNoShow
{
    public sealed record MarkReservationNoShowCommand( Guid ReservationId ):ICommand;
}
