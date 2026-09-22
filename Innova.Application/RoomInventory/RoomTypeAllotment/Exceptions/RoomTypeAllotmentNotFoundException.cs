using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.RoomTypeAllotment.Exceptions
{
    public sealed class RoomTypeAllotmentNotFoundException( RoomTypeAllotmentId allotmentId ):Exception($"Room type allotment '{allotmentId}' was not found.");
}
