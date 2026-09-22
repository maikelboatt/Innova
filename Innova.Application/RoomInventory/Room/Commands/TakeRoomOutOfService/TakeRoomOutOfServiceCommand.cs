using Innova.Application.Abstractions.Messaging;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.Room.Commands.TakeRoomOutOfService
{
    public sealed record TakeRoomOutOfServiceCommand( RoomId RoomId ):ICommand<Unit>;
}
