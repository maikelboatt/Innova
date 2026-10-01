using Innova.Application.RoomInventory.DTO;
using Innova.Infrastructure.RoomInventory.Room.Mapping;

namespace Innova.Infrastructure.RoomInventory.Room.Queries
{
    public static class RoomQueryHandlerMapper
    {
        public static IReadOnlyCollection<RoomDto> MapToRoomDto(
            IEnumerable<RoomRow> rooms ) => rooms
                                            .Select(ToDto)
                                            .ToList()
                                            .AsReadOnly();

        public static RoomDto ToDto( RoomRow roomRow ) => new(
            roomRow.Id,
            roomRow.RoomNumber,
            roomRow.FloorLevel,
            roomRow.FloorWing,
            roomRow.RoomTypeId,
            roomRow.Status);
    }
}
