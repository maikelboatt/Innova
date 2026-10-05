using Innova.Domain.FrontDesk.ValueObjects;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.ValueObjects;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Infrastructure.FrontDesk.Stay.Mapper
{
    public static class StayMapper
    {
        public static StayPersistenceModel ToPersistenceModel( Domain.FrontDesk.Aggregates.Stay stay )
        {
            Guid stayId = stay.Id.Value;

            StayRow stayRow = new()
                              {
                                  Id = stayId,
                                  ReservationId = stay.ReservationId.Value,
                                  GroupBookingId = stay.GroupBookingId?.Value,
                                  AssignedRoomId = stay.AssignedRoomId.Value,
                                  AssignedRoomNumber = stay.AssignedRoomNumber.Value,
                                  MaxOccupancy = stay.MaxOccupancy.Value,
                                  ActualCheckIn = stay.ActualCheckIn,
                                  ActualCheckOut = stay.ActualCheckOut
                              };

            IReadOnlyCollection<StayOccupantRow> occupantRows = ToOccupantRows(stay);

            return new StayPersistenceModel
                   {
                       Stay = stayRow,
                       Occupants = occupantRows
                   };
        }

        public static IReadOnlyCollection<StayOccupantRow> ToOccupantRows(
            Domain.FrontDesk.Aggregates.Stay stay )
        {
            return stay
                   .Occupants
                   .Select(guestId => new StayOccupantRow
                                      {
                                          StayId = stay.Id.Value,
                                          GuestId = guestId.Value
                                      })
                   .ToList()
                   .AsReadOnly();
        }

        public static Domain.FrontDesk.Aggregates.Stay ToDomain( StayRow row, IEnumerable<StayOccupantRow> occupantRows )
        {
            IReadOnlyCollection<GuestId> occupants =
                occupantRows
                    .Select(x => GuestId.From(x.GuestId))
                    .ToList()
                    .AsReadOnly();

            return Domain.FrontDesk.Aggregates.Stay.Reconstitute(
                StayId.From(row.Id),
                ReservationId.From(row.ReservationId),
                row.GroupBookingId.HasValue
                    ? GroupBookingId.From(row.GroupBookingId.Value)
                    : null,
                RoomId.From(row.AssignedRoomId),
                RoomNumber.Of(row.AssignedRoomNumber),
                MaxOccupancy.Of(row.MaxOccupancy),
                row.ActualCheckIn,
                row.ActualCheckOut,
                occupants);
        }
    }
}
