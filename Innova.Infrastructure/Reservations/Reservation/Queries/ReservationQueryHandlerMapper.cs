using Innova.Application.Reservations.DTO;
using Innova.Infrastructure.Reservations.Reservation.Mapping;

namespace Innova.Infrastructure.Reservations.Reservation.Queries
{
    public static class ReservationQueryHandlerMapper
    {
        public static ReservationDto ToDto( ReservationRow row ) => new(
            row.Id,
            row.GuestId,
            row.GroupBookingId,
            row.RoomTypeRequestedId,
            row.StayStart,
            row.StayEnd,
            row.NightlyRateAmount,
            row.NightlyRateCurrency,
            row.Status);

        public static IReadOnlyCollection<ReservationDto> MapToDto( IEnumerable<ReservationRow> rows ) => rows
            .Select(ToDto)
            .ToList()
            .AsReadOnly();

        public static ReservationSummaryDto ToSummaryDto( ReservationSummaryRow row ) => new(
            row.Id,
            row.GuestId,
            row.StayStart,
            row.StayEnd,
            row.Status);

        public static IReadOnlyCollection<ReservationSummaryDto> MapToSummaryDto( IEnumerable<ReservationSummaryRow> rows ) => rows
            .Select(ToSummaryDto)
            .ToList()
            .AsReadOnly();
    }
}
