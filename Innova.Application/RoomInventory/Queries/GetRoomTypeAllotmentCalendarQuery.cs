using Innova.Application.Abstractions.Messaging;
using Innova.Application.RoomInventory.DTO;

namespace Innova.Application.RoomInventory.Queries
{
    public sealed record GetRoomTypeAllotmentCalendarQuery(
        Guid RoomTypeId,
        DateOnly From,
        DateOnly To ):IQuery<IReadOnlyCollection<AllotmentNightDto>>;
}
