using Innova.Application.Abstractions.Messaging;
using Innova.Application.Reservations.DTO;

namespace Innova.Application.Reservations.Queries
{
    public sealed record GetReservationByIdQuery( Guid ReservationId ):IQuery<ReservationDto?>;
}
