using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.RoomTypeAllotment.Exceptions
{
    public sealed class NoAllotmentForDateException( RoomTypeId roomTypeId, DateOnly date ):ApplicationException(
        $"No capacity allotment exists for room type '{roomTypeId}' for {date}.");
}
