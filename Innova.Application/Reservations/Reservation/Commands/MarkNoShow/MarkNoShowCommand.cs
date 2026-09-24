using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Reservations.Reservation.Commands.MarkNoShow
{
    public sealed record MarkNoShowCommand( Guid ReservationId ):ICommand<Unit>;
}
