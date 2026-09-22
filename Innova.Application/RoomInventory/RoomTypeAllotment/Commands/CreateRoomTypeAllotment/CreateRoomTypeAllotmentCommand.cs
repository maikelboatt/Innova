using Innova.Application.Abstractions.Messaging;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.RoomInventory.RoomTypeAllotment.Commands.CreateRoomTypeAllotment
{
    public sealed record CreateRoomTypeAllotmentCommand(
        RoomTypeId RoomTypeId,
        DateRange StayPeriod,
        int TotalRooms ):ICommand<Guid>;
}
