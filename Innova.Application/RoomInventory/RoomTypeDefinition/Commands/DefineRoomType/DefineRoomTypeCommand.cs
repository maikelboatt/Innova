using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.RoomInventory.RoomTypeDefinition.Commands.DefineRoomType
{
    public sealed record DefineRoomTypeCommand(
        string RoomName,
        int MaxOccupancy,
        decimal BaseRateAmount,
        string BaseRateCurrency ):ICommand<Guid>;
}
