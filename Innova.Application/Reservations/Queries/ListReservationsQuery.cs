using Innova.Application.Abstractions.Messaging;
using Innova.Application.Reservations.DTO;

namespace Innova.Application.Reservations.Queries
{
    public sealed record ListReservationsQuery(
        Guid? GuestId,
        string? Status,
        DateOnly? ArrivingOn,
        DateOnly? DepartingOn ):IQuery<IReadOnlyCollection<ReservationSummaryDto>>;
}
