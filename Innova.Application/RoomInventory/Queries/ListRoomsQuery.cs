using Innova.Application.Abstractions.Messaging;
using Innova.Application.RoomInventory.DTO;

namespace Innova.Application.RoomInventory.Queries
{
    public sealed record ListRoomsQuery( string? Status, Guid? RoomTypeId, int? FloorLevel ):IQuery<IReadOnlyCollection<RoomDto>>;
}
