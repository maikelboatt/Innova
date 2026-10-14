using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.RoomInventory.RoomTypeDefinition.Commands.UpdateRoomType
{
    public sealed record UpdateRoomTypeCommand(
        Guid RoomTypeId,
        string RoomName,
        int MaxOccupancy,
        decimal BaseRateAmount ):ICommand;
}
