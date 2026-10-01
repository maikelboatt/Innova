using Innova.Application.RoomInventory.DTO;
using Innova.Infrastructure.RoomInventory.RoomType.Mapping;

namespace Innova.Infrastructure.RoomInventory.RoomType.Queries
{
    public static class RoomTypeQueryHandlerMapper
    {
        public static IReadOnlyCollection<RoomTypeDto> MapToRoomTypeDto( IEnumerable<RoomTypeRow> roomTypes ) => roomTypes
            .Select(ToDto)
            .ToList()
            .AsReadOnly();

        public static RoomTypeDto ToDto( RoomTypeRow roomTypeRow ) => new(
            roomTypeRow.Id,
            roomTypeRow.Name,
            roomTypeRow.MaxOccupancy,
            roomTypeRow.BaseRateAmount,
            roomTypeRow.BaseRateCurrency);
    }
}
