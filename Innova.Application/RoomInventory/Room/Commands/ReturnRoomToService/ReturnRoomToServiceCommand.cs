using Innova.Application.Abstractions.Messaging;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.Room.Commands.ReturnRoomToService
{
    public sealed record ReturnRoomToServiceCommand( RoomId RoomId ):ICommand<Unit>;
}
