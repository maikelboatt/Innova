using Innova.Application.Abstractions.Messaging;
using Innova.Application.RoomInventory.DTO;

namespace Innova.Application.RoomInventory.Queries
{
    public sealed record GetRoomByIdQuery( Guid RoomId ):IQuery<RoomDto?>;
}
