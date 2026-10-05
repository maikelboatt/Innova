using Innova.Application.FrontDesk.Stay.DTO;
using Innova.Infrastructure.FrontDesk.Stay.Mapper;

namespace Innova.Infrastructure.FrontDesk.Stay.Queries
{
    public static class StayQueryHandlerMapper
    {
        public static IEnumerable<StaySummaryDto> ToSummaryDto( IEnumerable<StaySummaryRow> stayRows )
        {
            return stayRows
                .Select(s => new StaySummaryDto(
                            s.Id,
                            s.AssignedRoomNumber,
                            s.ActualCheckIn));
        }

        public static StayDto ToDto( StayRow stayRow, IEnumerable<StayOccupantRow> occupants )
        {
            return new StayDto(
                stayRow.Id,
                stayRow.ReservationId,
                stayRow.AssignedRoomId,
                stayRow.AssignedRoomNumber,
                stayRow.MaxOccupancy,
                occupants
                    .Select(o => o.GuestId)
                    .ToList()
                    .AsReadOnly(),
                stayRow.ActualCheckIn,
                stayRow.ActualCheckOut,
                stayRow.GroupBookingId);
        }
    }
}
