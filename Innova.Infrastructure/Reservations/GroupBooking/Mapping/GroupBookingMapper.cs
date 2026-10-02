using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Infrastructure.Reservations.GroupBooking.Mapping
{
    public static class GroupBookingMapper
    {
        public static GroupBookingRow ToPersistenceModel( Domain.Reservations.Aggregates.GroupBooking groupBooking )
        {
            return new GroupBookingRow
                   {
                       Id = groupBooking.Id.Value,
                       OrganizerGuestId = groupBooking.OrganizerGuestId.Value,
                       GroupName = groupBooking.GroupName,
                       ReservationIds =
                       [
                           .. groupBooking
                              .ReservationIds.Select(r => r.Value)
                       ]
                   };
        }

        public static IReadOnlyCollection<Domain.Reservations.Aggregates.GroupBooking> ToDomain( IReadOnlyCollection<GroupBookingRow> rows ) => rows
            .Select(ToDomain)
            .ToList()
            .AsReadOnly();

        public static Domain.Reservations.Aggregates.GroupBooking ToDomain( GroupBookingRow row ) => Domain.Reservations.Aggregates.GroupBooking.Reconstitute(
            GroupBookingId.From(row.Id),
            GuestId.From(row.OrganizerGuestId),
            row.GroupName,
            row
                .ReservationIds.Select(ReservationId.From)
                .ToList()
                .AsReadOnly());
    }
}
