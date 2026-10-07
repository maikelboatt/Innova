using Innova.Application.Exceptions;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.RoomTypeAllotment.Exceptions
{
    public sealed class NoAllotmentForDateException( RoomTypeId roomTypeId, DateOnly date ):ApplicationExceptions(
        $"No capacity allotment exists for room type '{roomTypeId}' for {date}.");
}
