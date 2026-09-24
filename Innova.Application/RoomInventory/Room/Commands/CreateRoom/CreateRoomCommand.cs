using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.RoomInventory.Room.Commands.CreateRoom
{
    public sealed record CreateRoomCommand(
        string RoomNumber,
        int FloorLevel,
        string? Wing,
        Guid RoomTypeId ):ICommand<Guid>;
}
