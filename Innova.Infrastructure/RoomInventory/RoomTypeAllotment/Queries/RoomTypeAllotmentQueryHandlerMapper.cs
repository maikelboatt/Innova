using Innova.Application.RoomInventory.DTO;
using Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Mapping;

namespace Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Queries
{
    public static class RoomTypeAllotmentQueryHandlerMapper
    {
        public static IReadOnlyCollection<AllotmentNightDto> MapToAllotmentNightDto( IEnumerable<AllotmentNightRow> rows ) => rows
            .Select(ToDto)
            .ToList()
            .AsReadOnly();

        public static AllotmentNightDto ToDto( AllotmentNightRow row ) => new(
            row.Date,
            row.TotalRooms,
            row.BookedCount,
            row.Remaining);
    }
}
