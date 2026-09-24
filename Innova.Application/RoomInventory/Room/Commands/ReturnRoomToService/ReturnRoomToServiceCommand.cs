using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.RoomInventory.Room.Commands.ReturnRoomToService
{
    public sealed record ReturnRoomToServiceCommand( Guid RoomId ):ICommand<Unit>;
}
