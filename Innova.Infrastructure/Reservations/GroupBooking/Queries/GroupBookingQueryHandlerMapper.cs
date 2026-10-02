using Innova.Application.Reservations.DTO;
using Innova.Infrastructure.Reservations.GroupBooking.Mapping;

namespace Innova.Infrastructure.Reservations.GroupBooking.Queries
{
    public static class GroupBookingQueryHandlerMapper
    {
        public static GroupBookingDto ToDto( GroupBookingRow row ) => new(
            row.Id,
            row.OrganizerGuestId,
            row.GroupName,
            row.ReservationIds);

        public static IReadOnlyCollection<GroupBookingDto> MapToDto( IEnumerable<GroupBookingRow> rows ) => rows
            .Select(ToDto)
            .ToList()
            .AsReadOnly();
    }
}
