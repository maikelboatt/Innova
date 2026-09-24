using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.RoomInventory.Room.Commands.TakeRoomOutOfService
{
    public sealed record TakeRoomOutOfServiceCommand( Guid RoomId ):ICommand<Unit>;
}
