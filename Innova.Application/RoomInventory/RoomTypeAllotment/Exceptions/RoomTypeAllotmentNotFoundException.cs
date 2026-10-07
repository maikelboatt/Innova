using Innova.Application.Exceptions;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.RoomTypeAllotment.Exceptions
{
    public sealed class RoomTypeAllotmentNotFoundException( RoomTypeAllotmentId allotmentId ):ApplicationExceptions($"Room type allotment '{allotmentId}' was not found.");
}
