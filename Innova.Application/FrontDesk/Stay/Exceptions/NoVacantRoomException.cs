using Innova.Application.Exceptions;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.FrontDesk.Stay.Exceptions
{
    public sealed class NoVacantRoomException( RoomTypeId roomTypeRequested ):ApplicationExceptions($"No vacant room of type '{roomTypeRequested}' was found");
}
