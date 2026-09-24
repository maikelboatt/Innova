using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.RoomInventory.RoomTypeAllotment.Commands.CreateRoomTypeAllotment
{
    public sealed record CreateRoomTypeAllotmentCommand(
        Guid RoomTypeId,
        DateOnly CheckIn,
        DateOnly CheckOut,
        int TotalRooms ):ICommand<Guid>;
}
