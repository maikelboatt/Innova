using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.RoomInventory.RoomTypeAllotment.Exceptions
{
    public sealed class NoAllotmentForTypeAndPeriodException( RoomTypeId roomTypeId, DateRange stayPeriod ):ApplicationException(
        $"No capacity allotment exists for room type '{roomTypeId}' covering {stayPeriod.Start}–{stayPeriod.End}.");
}
